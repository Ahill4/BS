#SVG Creation Instructions

The original SVG images were all created in a program called inkscape( https://inkscape.org/). Which is essentially
a free version of adobe illustrator.  The svg that are made will require some specifications to work as intended with
the existing code. This readme will describe all of those things.

##Import

Begin by importing the provided floor plan. To do this you go to file import then browse to your file.  IF the file is an
image then there will be just that image when you import, but if you are provided with a pdf as was given for the originals
there will be extra objects to clean up later.

##Background

You can press (ctrl+shift+D) to open the document properties which is where you will need to edit the background size. Under 
custom size set width: 375 and height: 260, and the units to mm. Those values provide the extra space required to allow for movement in the div
container while zoomed in. Once your background is set up you will have to position your base floor plan in the standard spot. 
To do this in inkscape the top tool bar haas x, y, w, and h values you can set for the image. Select the base floor plan and set
X: 15, Y: 0, W: 240, and H: 149. For the future work of adjusting to screen size these values and the document properties values will likely
have to be changed.

##Base layer

If the floor plan was imported as an image then it will be of lower quality but there won't require cleaning up extra objects.
If it is imported from a pdf or something else as was given for the originals then you will have to clean up extra objects from the
source. To do this you can open the objects tab, there will be a list of specific objects. go through each element and use your judgement 
to decide if you should delete them. General rule, if you cant see the selection it can be deleted and if you can see the selection
then decide based on whether you think it is necessary.  Any text can be removed and any empyt box can be deleted, you can enable and disable
its visibility to determine if it has any effect on the image.

##Layers

The required layers are a base layer that will only contain the imported floor plan and 4 other layers. The layer order does not matter
but it would be nice to follow the same naming conventions. The 4 layers are Desk, Enclave, Social, and Meeting.  When placing locations
make sure you are placing them on the appropriate layers so that the right css can be applied.

##Locations

The locations must be made as either a rect or a path so that they can receive the hover effects from the css.  Just add the locations on the right 
layer and by tracing where it looks like they should go on the floor plan.  Also each location will need to be set with its own id which
can be set in the object properties tab. The id will have to follow a specific regex pattern of [A-Z][0-9]{4} for it work with the current code
also no 2 ids can be the same so you will have to keep track of what ids are provided and iterate accordingly. Addtionally the number immediatly 
following the letter in the regex should match the current floor you are working on. Also in the object properties you have the option to set the title.
i recommend setting the title to match the id as when you hover over the object in browser it will display the title allowing for easier debugging.

##Fill and Stroke

Once all objects are completed you will have to set specific fill and stroke to make sure that nothing is applied and they can take the css.
To do this open the fill and stroke tab then select all and under fill click on the element that looks like a question mark. This unsets the paint.
You also need to select the object that looks like a heart with a hole in it to make sure no intersections mess with coloring. Once those are
selected and you still have all the locations selected make sure the blur is set to 0 and the opacity is set to 100 on both the fill and stroke tab
and on the layers tab for each layer.

##Save and final edits

To save you can just go to file save but the name mmust follow the right format or there will be issues. The name must be Floor followed by the
floor number.svg (ex. Floor1.svg) the .svg will be automatically applied so you dont have to add it but make sure it is there. As for where to save it
it has to be saved in the project folder under "Views/Floors" if you are adding a floor that wasnt one of the originals (1, 4, 5, 6, 7) then a view will
have to be created. go look at the views under home and follow their format replacing the path with the new floor path.  Once it is saved in the proper
place with the proper format you now need to open it in a text editor and add the css classes to the appropriate layers. Do a (ctrl+f) and search for layer, 
go through each occurence of layer and add a class="class_name". the class names should be an all lower case version of the layer name if you followed the 
names given eariler.  Once all that is done all the styling should automatically apply and all that is left to do is to refill the database with the new 
locations. To do this you can go to the floor view in app and if you are logged in there will be a refill DB button that if you click will automatically
remove all the old locations from the DB and populate it with the new locations.
