<h2># Botox</h2>
dofus bot

Petite explication : https://cadernis.fr/index.php?threads/botox-mitm.2551/#post-25639

Pour lancer un hook c'est juste : </br>
  - 1 instance : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll");</code></pre> 
  
  - 10 instances : <pre><code class='language-cs'>HookManager.Instance.InitHook("VOTRE DOSSIER APP DOFUS", "./SocketHook.dll", 10); </code></pre> 
  
  (il y a un delay 3 secondes entre chaque ouverture de client donc ne soyez pas surpris) </br> 
