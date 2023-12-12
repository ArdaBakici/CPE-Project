using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataProcessing : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject heatmapPlane;
    public GameObject reactionBar; 

    public GameObject startPt, endPt;
    public TextMeshProUGUI resultText;

    public string gazeFileName = "gazeData.csv";
    public string responseFileName = "responseData.csv";
    public string quizFileName = "QASave.csv";
    
    // Gaze Score Parameters-----------
    public int[,] gazeBoxes = {{5, 10},
                               {25, 25},
                               {180, 180}}; 

    public float[] boxScores = {10, 5, 0};


    // Response Score Parameters--------
    [SerializeField]
    public Dictionary<string, ResponseEvent> responseEventWeights = new Dictionary<string, ResponseEvent>(){
        {"Ring", new ResponseEvent(0.2f, 2)},
        {"Sauron", new ResponseEvent(2f, 1)},
        {"Blade", new ResponseEvent(0.2f, 1)},
    };
    public float responseScoreFactor = 1; // per second
    public float responseTimeout = 10; // seconds
    public float defaultMinResponseTime = 0.2f; // In case ResponseEvent type is not recognized
    int total_num_of_events = 0;
    // Quiz Score Parameters------------
    public float correctPts = 1; // Number of points given to a correct answer
    public float difficultyFactor = 1; // Scaled by difficulty of question

    // Total Score Parameters-----------

    // Final Score Threshold: 
    public float scoreThreshold = 0; // If score is below this threshold, then the user is considered distracted

    // Weights for each score type: {gaze, response, quiz}
    public Dictionary<string, float> weights = new Dictionary<string, float>(){ 
        {"gaze", 1},
        {"response", 1},
        {"quiz", 1}
    };

    // Expected mean score for each type: {gaze, response, quiz}
    public Dictionary<string, float> expectedMeans = new Dictionary<string, float>(){ // Expected stdDev for each type: {gaze, response, quiz}
        {"gaze", 1},
        {"response", 1},
        {"quiz", 1}
    };

    // MAKE SURE THESE ARE IN THE SAME RATIO !!
    public int[] heatMapDims = {200, 200}; // Dimensions of heatmap
    public int[] maxMapAngles = {60, 60}; // Maximum angles of heatmap 
    float planeDist = 5.5f; // Distance of heatmap plane from camera
    public float brushStroke = 0.05f; // Brush stroke factor for heatmap (ratio to height)
    public float planeScaleFactor = 10; // Factor to scale heatmap plane by
    Texture heatMapTexture;


    string gazefile;
    string responsefile;
    string quizfile;

    float maxQuizScore = 0;
    float maxGazeScore = 0;
    void Start()
    {
        string path = Application.persistentDataPath;
        //string path = "Assets/Data/";
        gazefile = path + gazeFileName;
        responsefile = path + responseFileName;
        quizfile = path + quizFileName;

        processData(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void processData(){
        // Gaze Data processing
        float[][] gazeData = parseGazeData();
        float[] classifications = processGaze(gazeData);
        float[] gazeStats = calcDataStats(classifications, "Gaze");

        float[,] heatmap = createHeatMap(gazeData);
        drawHeatMap(heatmap);

        // Response Data processing
        float[][] responseData = parseResponseData();
        float[] responseScores = processResponseData(responseData);
        float[] responseStats = calcDataStats(responseScores, "Response Time");
        //resultText.text = responseStats[0]; 

        drawReactionTime(responseData);

        // Quiz Data processing
        float[][] quizData = parseQuizData();
        float[] quizScores = processQuizData(quizData);
        float[] quizStats = calcDataStats(quizScores, "Quiz");

        // Total Score Calculation
        float totalScore = calcTotalScore(gazeStats, responseStats, quizStats);

        // Displaying Data: 
        float avg = 0f;
        int len = 0;
        foreach(float[] s in responseData){
            len++;
            avg += s[0]; 
        }
        avg = avg/len;
        displayResults(gazeStats, responseStats, quizStats, totalScore);
    }


    // Gaze Processing Functions
    float[][] parseGazeData(){ // Reads data from gaze file and returns array of x, y angles

        float x, y; 
        List<float[]> gazeList = new List<float[]>(); 
        
        using(StreamReader sr = new StreamReader(gazefile)){
            string line;
            while((line = sr.ReadLine()) != null){
                if(line == ""){continue; } // Skip empty lines (last line
                string[] entries = line.Split(',');
                x = float.Parse(entries[0]);
                y = float.Parse(entries[1]);
                gazeList.Add(new float[]{x, y});
            }
        }

        float[][] gazeData = gazeList.ToArray();

        return gazeData; 
    }
    float[] processGaze(float[][] data){ // Classifies gaze data into boxes and returns array of classifications
        float[] classifications = new float[data.Length]; 
        for(int i = 0; i < data.Length; i++){
            classifications[i] = getGazeScore(data[i][0], data[i][1]);
        }
        return classifications;
    }
    float getGazeScore(float x, float y){ // Helper function for processGaze
 
        for(int i = 0; i < gazeBoxes.Length; i++){
            if(x < gazeBoxes[i,0] && y < gazeBoxes[i,1] && x > -gazeBoxes[i,0] && y > -gazeBoxes[i,1]){
                return boxScores[i]; 
            }
        }
        return -1; 
    }


    // Response Processing Functions
    float[][] parseResponseData(){ // Reads data from response file and returns array of response times
        List<float[]> respList = new List<float[]>();

        using(StreamReader sr = new StreamReader(responsefile)){
            string line;
            while((line = sr.ReadLine()) != null){
                if(line == ""){continue; } // Skip empty lines (last line
                string[] entries = line.Split(',');
                float responseTime = float.Parse(entries[1]);
                float minTime = responseEventWeights[entries[0]].minimumTime;
                respList.Add(new float[]{responseTime, minTime});
            }
        }
        float[][] responseData = respList.ToArray();
        return responseData; 
    }
    float[] processResponseData(float[][] data){ // Calculates response scores
        total_num_of_events = 0;
        foreach(KeyValuePair<string, ResponseEvent> response in responseEventWeights){
            //maxResponseScore += 1f/response.minimumTime * responseScoreFactor * response.numberOfEvents;
            total_num_of_events += response.Value.numberOfEvents;
        }
        
        float[] scores = new float[data.Length]; // by default array values are 0 
        for(int i = 0; i < data.Length; i++){
            scores[i] = getResponseScore(data[i]);
        }
        return scores; 
    }
    float getResponseScore(float[] response){
        float score = 0;
        if(response[0] > responseTimeout){
            // user time is infinity lim(x->inf) 1/sqrt(x) = 0
            score = 0;
        }
        else{
            if(response[0] < response[1]) response[0] = response[1];
            // we use sqrt because it scales better than 1/x
            score = Mathf.Sqrt(response[1]/response[0]) * responseScoreFactor;
        }
        return score;
    }
 

    // Quiz Processing Functions
    float[][] parseQuizData(){ // Reads data from quiz file and returns array of {correct, difficulty}
        List<float[]> quizList = new List<float[]>();

        using(StreamReader sr = new StreamReader(quizfile)){
            string line;
            while((line = sr.ReadLine()) != null){
                if(line == ""){continue; } // Skip empty lines (last line
                string[] entries = line.Split(',');
                float correct = float.Parse(entries[0]);
                float difficulty = float.Parse(entries[1]);
                quizList.Add(new float[]{correct, difficulty});
            }
        }
        float[][] quizData = quizList.ToArray();
        return quizData; 
    }
    float[] processQuizData(float[][] data){ // Calculates quiz scores
        float[] scores = new float[data.Length]; 
        for(int i = 0; i < data.Length; i++){
            scores[i] = getAnsScore(data[i][0], data[i][1]);
        }
        return scores; 
    }
    float getAnsScore(float correct, float difficulty){
        float score = correct * correctPts  *  difficulty * difficultyFactor;

        maxQuizScore += correctPts * difficulty * difficultyFactor;
        return score;
    }


    // General statistics calculator 
    float[] calcDataStats(float[] data, string type){ // Returns Array: {average, stdDev, totalScore, numDataPoints}
        float avg = 0;
        int len = data.Length; 
        foreach(float s in data){
            
            if(s == -1){ // If Invalid, then don't count it in average
                len--; 
                continue; 
            }

            avg += s; 
        }
        avg = avg/len;

        float stdDev = 0;
        foreach(float s in data){
                if(s == -1){ // Event Condition: If timeout, then don't count it in average
                continue; 
            }
            stdDev += Mathf.Pow(s - avg, 2); 
        }
        stdDev = Mathf.Sqrt(stdDev/len);

        Debug.Log(type + " | Average Score: " + avg);
        Debug.Log(type + " | Std Dev: " + stdDev);

        float[] res = {avg, stdDev, data.Sum(), data.Length};

        return res; 
    }


    // Total Score Calculator
    float calcTotalScore(float[] gazeStats, float[] responseStats, float[] quizStats){

        maxGazeScore = boxScores.Max() * gazeStats[3];

        // Normalize weights
        float normGazeWeight = weights["gaze"] / weights.Values.Sum();
        float normResponseWeight = weights["response"] / weights.Values.Sum();
        float normQuizWeight = weights["quiz"] / weights.Values.Sum();

        float totalScore = 0;

        // Gaze Score
        float gazeScore = gazeStats[2] / maxGazeScore * normGazeWeight;
        totalScore += gazeScore;

        // Response Score
        float responseScore = responseStats[2] / total_num_of_events * normResponseWeight;
        totalScore += responseScore;

        // Quiz Score
        float quizScore = quizStats[2] / maxQuizScore * normQuizWeight ;
        totalScore += quizScore;
    
        return totalScore;  

    }



    float[,] createHeatMap(float[][] data){
        // Create heatmap of gaze data
        double maxX = Math.Tan(maxMapAngles[0] * Mathf.Deg2Rad) * planeDist;
        double maxY = Math.Tan(maxMapAngles[1] * Mathf.Deg2Rad) * planeDist;

        heatMapDims[0] = (int)Math.Round(maxX * planeScaleFactor); 
        heatMapDims[1] = (int)Math.Round(maxY * planeScaleFactor);    

        float[,] heatMap = new float[heatMapDims[1], heatMapDims[0]];

        
        Debug.Log("Max x and y: " + maxX + " " + maxY);
        Debug.Log("Heatmap dims: " + heatMapDims[0] + " " + heatMapDims[1]); 

        foreach(float[] d in data){
            float thetaX = d[0];
            float thetaY = d[1];

            if(thetaX > maxMapAngles[0] || thetaY > maxMapAngles[1]){
                continue;
            }   

            double x = Math.Tan(thetaX * Mathf.Deg2Rad) * planeDist;
            double y = Math.Tan(thetaY * Mathf.Deg2Rad) * planeDist;            


            int xInd = (int) Math.Round( (x/(maxX) + 1)/2 * heatMapDims[0]) ;
            int yInd = (int) Math.Round( (-y/(maxY) + 1)/2 * heatMapDims[1]) ;

            // Debug.Log(x + " " + y + " " + xInd + " " + yInd); 

            updateMapValues(heatMap, xInd, yInd);

        }
        normalizeMap(heatMap);
        return heatMap;

    }

    void updateMapValues(float[,] map, float x, float y){

        int minBoundX, maxBoundX, minBoundY, maxBoundY;
        
        float brushSize = heatMapDims.Min() * brushStroke; 

        // Setting Brush Stroke Bounds
        if( x - brushSize <0){ minBoundX = 0; }
        else{ minBoundX = (int) (x - brushSize);}

        if( x + brushSize > heatMapDims[0]){maxBoundX = heatMapDims[0];}
        else{maxBoundX = (int) (x + brushSize);}

        if( y - brushSize <0){minBoundY = 0;}
        else{minBoundY = (int) (y - brushSize);}

        if( y + brushSize > heatMapDims[1]){maxBoundY = heatMapDims[1];}
        else{maxBoundY = (int) (y + brushSize);}

        // Debug.Log(minBoundX + " " + maxBoundX + " " + minBoundY + " " + maxBoundY); 

        // Updating Map Values: Add values in a circle around the point that decreases in magnitude with distance from point
        for(int i = minBoundY; i < maxBoundY; i++){
            for(int j = minBoundX; j < maxBoundX; j++){
                float dist = Mathf.Sqrt(Mathf.Pow(x-i, 2) + Mathf.Pow(y-j, 2));
                float val = dist!= 0 ? 1/dist : 1; 
                // Debug.Log((i >= heatMapDims[1]) + " " + (j >= heatMapDims[0])); 
                map[i,j] += val;
            }
        }

    }

    void normalizeMap(float[,] map){
        float maxVal = map.Cast<float>().Max();
        for(int i = 0; i < heatMapDims[1]; i++){
            for(int j = 0; j < heatMapDims[0]; j++){
                map[i,j] = map[i,j]/maxVal;
            }
        }
    }

    void drawHeatMap(float[,] map){ // ONLY WORKS FOR SQUARE MAPS

        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(Color.blue, 0.0f), 
                                    new GradientColorKey(Color.green, 0.3f),
                                    new GradientColorKey(Color.yellow, 0.6f) , 
                                    new GradientColorKey(Color.red, 1.0f) },

            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) }
        );


        heatmapPlane.transform.localScale = new Vector3((float)heatMapDims[0]/heatMapDims.Max(), (float)heatMapDims[1]/heatMapDims.Max(), 1);
        var texture = new Texture2D(heatMapDims[0], heatMapDims[1]);

        Color[,] colors = new Color[heatMapDims[1] , heatMapDims[0]];

        for(int i = 0; i < heatMapDims[1]; i++){
            for(int j = 0; j < heatMapDims[0]; j++){
                float val = map[i,j];
                colors[i, j] = gradient.Evaluate(val);
            }
        }

        Color[] pixelColors = new Color[heatMapDims[0] * heatMapDims[1]];
        int index = 0; 
        for(int j=0; j < heatMapDims[0]; j++){
            for(int i= heatMapDims[1] - 1; i >= 0; i--){
                pixelColors[index++] = colors[i, j];
            }
        }

        texture.SetPixels(pixelColors);
        // texture.SetPixels(colors.Cast<Color>().ToArray());

        texture.Apply();
        heatmapPlane.GetComponent<Renderer>().material.mainTexture = texture;

    }

    void drawReactionTime(float[][] data){
        float avg = 0; 
        int len = data.Length; 
        for(int i = 0; i < data.Length; i++){
            avg += data[i][0]; 
            if(data[i][0] > responseTimeout){ // Event Condition: If timeout, then don't count it in average
                len--; 
            }
        }   
        avg = avg/len * 1000;

        Vector3 barStartPos = startPt.transform.localPosition; 
        Vector3 barEndPos = endPt.transform.localPosition;

        float minResTime = 0; 
        float maxResTime = 475;

        Vector3 newBarPos = (barEndPos - barStartPos) * (avg - minResTime) / (maxResTime - minResTime) + barStartPos;
        Debug.Log(newBarPos);

        // reactionBar.transform.position = reactionBar.transform.TransformVector(newBarPos);
        Debug.Log(avg); 
        reactionBar.transform.localPosition = newBarPos;


    }

    void displayResults(float[] gazeStats, float[] responseStats, float[] quizStats, float totalScore){
        string printTxt = "\n";
        float result = calcADHDProb(totalScore);
        printTxt += "Gaze Score: " + gazeStats[2] + " Max : " + maxGazeScore + "\n";
        printTxt += "Response Score: " + responseStats[2]/total_num_of_events + "\n";
        printTxt += "Quiz Score: " + quizStats[2] + "\n";
        printTxt += "Total Score: " + totalScore + "\n";
        printTxt += "ADHD Probability: " + (100 - (result * 100)) + "%";

        resultText.text = printTxt;

    }

    float calcADHDProb(float score){
        float prob; 
        if(score - scoreThreshold < 0){
            prob = 0.5f * (score/scoreThreshold);
        }
        else{
            prob = 0.5f + 0.5f * (score - scoreThreshold)/(1-scoreThreshold);
        }
        
        return (float)Math.Round(prob, 3);  

    }

}

public class ResponseEvent{
    public float minimumTime; // in seconds
    public int numberOfEvents; // how many times event occurs

    public ResponseEvent(float minTime, int numOfEvent){
        minimumTime = minTime;
        numberOfEvents = numOfEvent;
    }
}

// Give bonus to responsetimeout? 
// Find appropriate methods for avg and stdDev classification 
// 