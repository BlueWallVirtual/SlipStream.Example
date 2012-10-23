This is an example dynamic module for Robust used with the 

Here is the list of current features...

*dynamic/modular configuration - The application will look in the default ini for its configuration section. If none is found, it will look in a named directory for its modular config file. If no file is found, it will, then, look in a url provided in the module for a "bootstrap" ini that will be downloaded and written to the specified ini.

*remote repositories - Remote repositories may be managed from the console. A repository may be added, enabled, disabled or removed. When a registered repository is enabled, suitable plugins for the application will be listed when searching for available plugins.

*dynamic plugins - Registering a repository and searching for available plugins will list possible candidates for extending the application. The plugins may be added and removed from the application. After adding a plugin to the application, it may enabled or disabled. It's meta data may also be viewed. Plugins contained in the local ./bin directory are immediately available, but should not be included in the ServiceConnectors list.


** Testing:

get the code: OpenSim repo - connector_plugin branch

Build the code as usual and run an instance of Robust on a convenient port. Note RegistryLocation and ConfigDirectory in the [Startup] section of your Robust.ini and set them appropriately.

see the commands with "help repository" and "help plugin"

I have setup an example repository that can be used for testing. Add it with...

repo add http://bluewallvirtual.com/plugins


Then, look for available plugins with...

plugin list available


Then, add a plugin...

plugin add 0


Then, you can enable the plugin to have it get the bootstrap ini...

plugin enable 0


At this point, the plugin will be disabled by configuration, so disable it in the console...

plugin disable 0


Now find your new configuration file and edit it for your needs. When you have finished editing your ini enable the plugin...

plugin enable 0


Now you should be able to use it. You can also see the meta data...

plugin info 0


You can check the state of your installed plugins...

plugin list installed

Your plugin will show: 0) [ ] ...

plugin disable 0
plugin list installed

Your plugin will show: 0) [X] ...


You can remove the plugin...

plugin remove 0


Config file: http://bluewallvirtual.com/configs/SlipStream.Example.xml

To make a repository...

0) Need "mautil" from the mono-addins package
1) Prepare a place to serve your configs and repository on your web server
2) Edit the metadata and the ConfigURL property in SlipStreamConnector.cs
3) Build your plugin in the addon-modules directory like a region module
4) Pack the dll: mautil p bin/MyPlugin.dll
5) Copy all packed files to a directory, then cd to that directory
6) Build the repo: mautil rb ./
7) Copy the directory contents to your repository on your web server
8) Copy config xml to your config url on your web server

Clean your registry by deleting the directory pointed to in "RegistryLocation" in your Robust.ini. You're all set.

