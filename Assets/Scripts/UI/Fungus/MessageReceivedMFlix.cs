using UnityEngine;
using Fungus;

[EventHandlerInfo("Scene",
                      "Message Received (Maujflix)",
                      "The block will execute when the specified message is received from a Send Message command.\n\n **--> Modified for Character Selection Screen with Diamond Integration <--**")]
public class MessageReceivedMFlix : MessageReceived
{
    [SerializeField] protected CharacterSelectionScreen characterSelectionScreen;

    public void OnFungusMessageReceivedForCharacter(string _receivedMessage, CharacterSelectionScreen _characterSelectionScreen)
    {
        if(_receivedMessage == message && characterSelectionScreen == _characterSelectionScreen)
        {
            ExecuteBlock();
        }
    }
}