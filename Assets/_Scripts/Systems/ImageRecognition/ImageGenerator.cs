using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Systems.ImageRecognition
{
    public class ImageGenerator
    {
        [System.Serializable]
        public class OpenAIRequest
        {
            public string model;
            public string prompt;
            public int n;
            public string size;
        }

        [System.Serializable]
        public class OpenAIResponse
        {
            public string created;
            public OpenAIData[] data;
        }
        
        [System.Serializable]
        public class OpenAIData
        {
            public string revised_prompt;
            public string url;
        }

        private const string apiKey = ""; // Reemplaza con tu API key de OpenAI
        private const string apiUrl = "https://api.openai.com/v1/images/generations";

        public async Task<byte[]> GenerateImageAsync(string inputPrompt)
        {
            var requestBody = new OpenAIRequest
            {
                model = "dall-e-3",
                prompt = inputPrompt,
                n = 1,
                size = "1024x1024"
            };

            var jsonData = JsonUtility.ToJson(requestBody);
            Debug.Log("Generating image with data: " + jsonData);

            return await SendRequestAsync(jsonData);
        }

        private async Task<byte[]> SendRequestAsync(string jsonData)
        {
            var request = new UnityWebRequest(apiUrl, "POST");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("User-Agent", "Unity");

            // wait for the request to finish knowing that Type 'UnityEngine.Networking.UnityWebRequestAsyncOperation' is not awaitable
            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
                return null;
            }
            Debug.Log("Request sent successfully!");
            var response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
            var imageUrl = response.data[0].url;
            return await DownloadImage(imageUrl);
        }
        private static async Task<byte[]> DownloadImage(string imageUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return null;
            }
            Debug.Log("Image downloaded successfully!");
            var texture = DownloadHandlerTexture.GetContent(request);
            // return the texture to the caller texture.EncodeToPNG();
            return texture.EncodeToPNG();
        }
    }
}
