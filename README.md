# Image Search Example

## Built with Planet Unity

This is a moderate example of how to build Unity UI using [Planet Unity](https://github.com/SmallPlanetUnity/PlanetUnity2); it creates an application which allows you to perform an image by entering some text in an input field, pressing the search button, performing some asynchronous web requests and populating a grid table with the resulting images.



![kittens!](https://github.com/SmallPlanetUnity/GoogleImageSearch/raw/master/kittens.png)




## Reconstructing from scratch

While having access to the completed project is nice, it can be much more informative to build it yourself from scratch. Here are the step-by-step instructions on how to duplicate this project on your own:


# Creating and preparing the Unity project

* Open Unity and create a new project
* Create a Assets/Resources/Search folder
* Create a Assets/Code folder
* Create a Assets/Scenes folder
* Create a new scene and save it as Assets/Scenes/Search.unity
* Follow the [Planet Unity integration instructions](https://github.com/SmallPlanetUnity/PlanetUnity2/blob/master/Documentation/GettingStarted_Integration.md). I git-submoduled PlanetUnity to Assets/PlanetUnity2

# Creating the basic UI of the Search scene
* Create a new XML file Assets/Resources/Search/Search.xml
* Add the following XML to Search.xml. This will provide a basic Canvas and a white square set to cover the entire screen.

 ````
	<?xml version="1.0" encoding="utf-8" ?>
	<Canvas renderMode="ScreenSpaceCamera" xmlns="http://schema.smallplanet.com/PlanetUnity2.xsd" >
	    <Color color="#FFFFFFFF" anchor="stretch,stretch" />
	</Canvas>
````

* In the Search.unity scene, you should have created a Game Object and attached the Planet Unity Game Object component to it.  In the XmlPath field, add the path "Search/Search" (that's the Assets/Resources/Search/Search.xml path)
* At this point, you should be able to play the Unity Editor and see the canvas and white square in the object hierarchy.

# Creating a class to provide project specific overrides to Planet Unity

* Planet Unity provides a lot of overrides and customizations to help fit your project needs. For this project, we need to override the default image loading optimization to allow Planet Unity to load Sprites.
* Create a new C# script in Assets/Code/PlanetUnityOverrides.cs, add the following code to the file:

 ````
	using UnityEngine;
	public class PlanetUnityOverrides : MonoBehaviour, IPUCode {
	
		public void Awake() {
			PlanetUnityOverride.ForceActualSprites = true;
		}
	}
````

* When setting overrides, it is important to set them in the Awake method. The Planet Unity Game Object component will load everything in the Start method, so this will ensure your overrides are properly set before any of the UI is loaded.
* Add this new component to your the game object which contains your the Planet Unity Game Object component (technically it can be on any game object).

# Creating the search field and search button

* Add the following images ( searchButton.png, searchIcon.png, and inputFieldBackground.png ) to the Assets/Resources/Search folder
* For the searchButton.png and the inputFieldBackground.png we want them to be Sprites and their border values set to 31,31,31,31 (this will allow them nicely to 9-point stretch to fill the space)
* In Search.xml, add the the input field after the background color:

 ````
	<!-- This is a gray bar going along the top of the screen ; this will eventually cover the grid of images when they scroll up under the input field -->
	<Color color="#DDDDDDFF" bounds="@eval(0,0,w,68)" pivot="0,1" anchor="top,stretch" />
	<!-- This is the background images for the input field -->
	<Image resourcePath="Search/inputFieldBackground" bounds="10,-10,700,48" pivot="0,1" anchor="top,stretch">
		<!-- This is the actual input field component -->
		<InputField title="SearchField" onValueChanged="PerformSearch" sizeToFit="true" maxFontSize="28" alignment="middleLeft" bounds="@eval(10,10,w-20,h-20)" anchor="stretch,stretch" />
		<!-- This is the blue search button, attached to the right side of the text field -->
		<ImageButton resourcePath="Search/searchButton" onTouchUp="PerformSearch" bounds="@eval(0,0,48,48)" pivot="0,0.5" anchor="middle,right">
			<Image resourcePath="Search/searchIcon" pivot="0.5,0.5" bounds="@eval(0,0,w-20,h-20)" anchor="middle,center" />
		</ImageButton>
	</Image>
````

* At this point, you should be able to Play the editor and see the gray bar along the top, the input field, and the blue search button

# Adding the grid table and the controller class

* When the results of the image search requests comes in, we will place those results in a nice scrolling table for the user to browse through.  Add the following to Search.xml, right above the gray bar entity.

 ````
<GridTable title="ImageResultsTable" expandToFill="false" bounds="@eval(0,0,w,h-68)" anchor="stretch,stretch" />
````

* Before we write a C# script to control this scene, we want to attach it to this scene using a Code entity. Add the following lines to the bottom of Search.xml. When Planet Unity loads the XML, it will create an instance of the SearchController class, and link any titled entities to that classes member variables.

 ````
	<Code class="SearchController">
		<Notification name="PerformSearch" />
	</Code>
````


* Now that the grid table and the input field is in place, we need to write some controller code to take the input from the text field, run the image search, and output the resulting images into the grid table. Create a C# script file at Assets/Code/SearchController.cs and put the following script into it:

 [SearchController.cs](https://github.com/SmallPlanetUnity/GoogleImageSearch/blob/master/Assets/Code/SearchController.cs)

1. The public member variables which match the names and types of the titled entities will be populated automatically on creation of the controller class instance.
2. This code relies on litjson for the JSON parsing; you should include litjson or another JSON parser to handle this.
3. This is a standard Unity MonoBehaviour subclass, and it added as a comoponent to the game object which gets created by the Code entity
4. When you end editing on the input field, or press the search button, a code notification is sent named **PerformSearch**. In the previous step in our Code entity we have a Notification entity, whose name is **PerformSearch**. This tells the code entity that it should listen for notifications of **PerformSearch** and when they happen, call the PerformSearch() method on the SearchController class.

# Code to support using PUTables

* To use any variant of PUTable (PUTable, PUGridTable, or PUSimpleTable) we need to do a little bit more work; we need to create table cell classes for each type of table cell class we want to the table to be able to hold
* These classes, along with some image downloading code which uses Unity's WWW class and coroutine to download the images asynchronously, are in the ImageTableSupport.cs

 [ImageTableSupport.cs](https://github.com/SmallPlanetUnity/GoogleImageSearch/blob/master/Assets/Code/ImageTableSupport.cs)

1. To use PUTable, you supply a list of objects ( List&lt;object&gt;() ) to the entity by calling SetObjectList().
2. The list should be a list of data classes which contain the data for each cell (in our example, the **ImageResult** class )
3. We need to create a subclass of PUTableCell named the same name as our data class + TableCell (in our example **ImageResultTableCell**). This class is responsible to creating the view of each cell (either programmatically or by loading a table cell XML file), and being the controller for this cell (putting the actual data for each cell in the cell's views).

# All done

At this point you should be able to play the Unity Editor, enter a search text in the input field, press the search button, and see the image search results populate the grid table.


## License

PlanetUnity is free software distributed under the terms of the MIT license, reproduced below. PlanetUnity may be used for any purpose, including commercial purposes, at absolutely no cost. No paperwork, no royalties, no GNU-like "copyleft" restrictions. Just download and enjoy.

Copyright (c) 2014 [Small Planet Digital, LLC](http://smallplanet.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

## About Small Planet

Small Planet is a mobile agency in Brooklyn, NY that creates lovely experiences for smartphones and tablets. PlanetUnity has made our lives a lot easier and we hope it does the same for you. You can find us at www.smallplanet.com. 