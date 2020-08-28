<h2># Botox</h2>
dofus bot

Petite explication : https://cadernis.fr/index.php?threads/botox-mitm.2551/#post-25639

Pour lancer un hook c'est juste : </br>
  - 1 instance : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll");</code></pre> 
  
  - 10 instances : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll", 10); </code></pre> 
  
  (vous pouvez définir le delay d'ouverture entre chaque client dans le fichier startup.json) </br> 

<h2> Configuration </h2>

startup.json dans le dossier de l'éxécutable ( créer automatiquement lors de la première ouverture donc pas besoin de le faire manuellement )
```json
{
  "dofus_location": "D:/DofusApp/Dofus.exe",
  "dll_location": "./SocketHook.dll",
  "client_count": 1,
  "time_wait_in_ms": 2000,
  "show_message": true,
  "show_message_content": false,
  "show_data": false,
  "show_fake_message_sent": true,
  "show_ui": false
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

<h2> Interface Graphique </h2>

Pour l'instant l'interface graphique n'est qu'à ses bases , je compte y ajouter d'autres fonctionalité pour plus tard. Elle me permet juste de mieux voir certaine données pour l'instant.
