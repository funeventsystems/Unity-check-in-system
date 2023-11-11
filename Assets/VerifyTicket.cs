using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class VerifyTicket : MonoBehaviour
{
    public InputField ticketIdInput;
    public Button verifyButton;
    public Image successScreen;
    public Image failScreen;
    public Text messageText; // Reference to your Text component

    public string baseUrl = "https://tickets.mastermindsyyc.xyz/api/verifyticket"; // Base URL
    private string ticketId;

    private void Start()
    {
        // Add a listener to the button
        verifyButton.onClick.AddListener(VerifyTicketClicked);
    }

    private void VerifyTicketClicked()
    {
        ticketId = ticketIdInput.text;
        Debug.Log("Ticket ID entered: " + ticketId);

        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
        // Build the complete URL
        string url = baseUrl + "/" + ticketId;
        Debug.Log("Sending POST request to URL: " + url);

        // Create a JSON object to send data with the POST request
        var requestData = new Dictionary<string, string>();
        requestData["ticketId"] = ticketId;

        // Convert the JSON data to a string
        string jsonRequestBody = JsonUtility.ToJson(requestData);

        // Create a UnityWebRequest to send the POST request
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Request failed. Error: " + www.error);

            if (www.responseCode == 400)
            {
                // Handle 400 Bad Request error (change screen color to red)
                failScreen.gameObject.SetActive(true);
                successScreen.gameObject.SetActive(false);
                messageText.text = "Error: " + www.error;
                StartCoroutine(ResetScreensAfterDelay(2f));
            }
            else
            {
                // Handle other network or HTTP errors
                // (You can add specific handling for different error codes here)
                Debug.LogError("Network/HTTP error: " + www.responseCode);
                StartCoroutine(ResetScreensAfterDelay(2f));
            }
        }
        else
        {
            Debug.Log("Request was successful. Response: " + www.downloadHandler.text);
            // Request was successful (change screen color to green)
            successScreen.gameObject.SetActive(true);
            failScreen.gameObject.SetActive(false);
            messageText.text = "Response: " + www.downloadHandler.text;
            StartCoroutine(ResetScreensAfterDelay(2f));
        }
    }

    IEnumerator ResetScreensAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Resetting screens after " + delay + " seconds.");
        // Reset the screens
        successScreen.gameObject.SetActive(false);
        failScreen.gameObject.SetActive(false);
        messageText.text = ""; // Clear the message text
    }
}
