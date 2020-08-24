<h2># Botox</h2>
dofus bot

Petite explication : https://cadernis.fr/index.php?threads/botox-mitm.2551/#post-25639

Pour lancer un hook c'est juste : </br>
  - 1 instance : ``csharp HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll"); 
  
  - 10 instances : ``csharp HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll", 10); 
  
  (il y a un delay 3 secondes entre chaque ouverture du client donc ne soyez pas surpris) </br> 
