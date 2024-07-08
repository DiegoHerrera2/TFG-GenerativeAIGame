using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Systems.ImageRecognition
{
    public class ImageRecognizer
    {

        private const string APIUrl =
            "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key=";

        [System.Serializable]
        public class Response
        {
            public Candidate[] candidates;
        }
        
        [System.Serializable]
        public class Candidate
        {
            public Content content;
            public string finishReason;
            public string index;
            public SafetyRatings safetyRatings;
        }
        
        [System.Serializable]
        public class Content
        {
            public Parts[] parts;
            public string role;
        }
        
        [System.Serializable]
        public class Parts
        {
            public string text;
        }
        
        [System.Serializable]
        public class SafetyRatings
        {
            public string category;
            public string probability;
        }
        
        public async Task<List<string>> AnalyzeImageAsync(byte[] imageData)
        {

            // Create JSON request
            var jsonRequest = @"
            {
                ""contents"": [
                    {
                        ""parts"": [
                               {""text"": ""Return a list, and only the list, of labels that tells whats the object in the image. For example, if it is a sun, you must response
                                with the following list:sun, sunlight, light, star, fire... and so on. Just write the labels separated by commas.""},
                                {""inlineData"": {
                                    ""mimeType"": ""image/png"",
                                    ""data"": """ + System.Convert.ToBase64String(imageData) + @"""
                                    }
                                }
                        ]
                    }
                ]
            }";
            using UnityWebRequest www = new UnityWebRequest(APIUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Send request
            var asyncOperation = www.SendWebRequest();
            float timeCounter = 0;
            while (!asyncOperation.isDone)
            {
                timeCounter += Time.deltaTime;
                
                if (timeCounter > 10)
                {
                    Debug.LogError("Request timed out");
                    return null;
                }
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request failed: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);
                return null;
            }
            // Parse response
            string jsonResponse = www.downloadHandler.text;
            var response = JsonUtility.FromJson<Response>(jsonResponse);
            var labels = response.candidates[0].content.parts[0].text.Split(',');
            List<string> LabelsDetected = new List<string>();
            foreach (var label in labels)
            {
                LabelsDetected.Add(label.Trim());
            }
            return LabelsDetected;
        }
    }
}
