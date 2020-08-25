<h2># Botox</h2>
dofus bot

Petite explication : https://cadernis.fr/index.php?threads/botox-mitm.2551/#post-25639

Pour lancer un hook c'est juste : </br>
  - 1 instance : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll");</code></pre> 
  
  - 10 instances : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll", 10); </code></pre> 
  
  (il y a un delay 3 secondes entre chaque ouverture de client donc ne soyez pas surpris) </br> 

<h2> Configuration </h2>

startup.json dans le dossier de l'éxécutable ( créer automatiquement lors de la première ouverture donc pas besoin de le faire manuellement )
```json
{
  "dofus_location": "D:/DofusApp/Dofus.exe",
  "dll_location": "./SocketHook.dll",
  "client_count": 1,
  "show_message": true,
  "show_message_content": false,
  "show_data": false,
  "show_fake_message_sent": true
}
```

<h2> Handler </h2>

voici un exemple de handler

```csharp
public class CustomHandler : IMessageHandler
{
    // ici vous pouvez mettre soit l'id du message , soit le nom du message
    [Handler(1)]
    // proxy element contient le client local ( Client ) et le client lié au serveur ( FakeClient )
    // proxy.SendServer() -> envoyer un message au server
    // proxy.SendClient() -> envoyer un message au client
    public void HandleProtocolRequiredMessage(ProxyElement proxy, NetworkElementField message, ProtocolJsonContent content)
    {
        Console.WriteLine($"Test : {content["requiredVersion"]}");
    }
}
``` 
Votre class doit dériver de IMessageHandler , il n'y a pas de restriction sur le nom de votre fonction , mais celui-ci doit respectez le même format que l'exemple un peu plus haut ( ne pas oublier l'attribut, et les arguments de la fonction  )
