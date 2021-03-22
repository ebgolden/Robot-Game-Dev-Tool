# Overview
    
This is a development tool for making parts for The Robot Games. Part files you make can be found in the parts directory.

# Usage
The only accepted image type for part images is .png files and the recommended size for all images is 512x512 px. There are 3 ways that image files should be formatted so they look the way you intend for your part:

1. Cube image. Used for parts with rectangular prism shape. Image is split into 6 regions for each face of part. Top third of image should be transparent (rgba(0, 0, 0, 0)). Look at body.png and mobility.png in "examples" directory.
    - The arrangement of the sides looks like this:\
                        B  &nbsp;|  &nbsp;L  &nbsp;|  &nbsp;R       &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Back  &nbsp;|  &nbsp;Left  &nbsp;|  &nbsp;Right\
                        ---------------   &nbsp; &nbsp; = &nbsp; &nbsp;   -----------------------------------------\
                        F  &nbsp;|  &nbsp;T  &nbsp;|  &nbsp;S       &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Front  &nbsp;|  &nbsp;Top  &nbsp;|  &nbsp;Stern(Back)
2. Sphere image. Used for parts with hemisphere shape. Image is spread from back to from, horizontally, across file and front of image is offset 32 px from right edge of file. Bottom half of image should be transparent (rgba(0, 0, 0, 0)). Look at head.png in "examples" directory.
3. Attachment image. Used for attachments parts (any part that is not a Head, Body, or Mobility. e.g. Blaster). Image is not formatted in any special way, but parts of file that should not be part of image should be transparent (rgba(0, 0, 0, 0)). Look at blaster.png in "examples" directory.

# Notes

* You can also edit parts that you have already made. However, you can not edit parts that other people have made unless you have their createdparts\*.roga file (\* = character that may vary).
* While there are no plans to update this tool, new versions of the file "typedatap.roga" will be uploaded to the source repo. Simply replace your version of the file with the latest one so you have the latest part type and effect options.
* If you have The Robot Games installed at a directory level adjacent to the "Robot Game Dev Tool" directory, then you can add your parts directly into the game by clicking the "Add to Game" button. Note: if you edit a part you have made that is currently in your game, you will still have to click the "Add to Game" button to update the part file.
* Part files you create are stored in the Robot Game Dev Tool "parts" directory. This is useful if you want to send friends your part file or if you have The Robot Games installed in a different location and have to manually copy your par files to The Robot Games' "parts" directory.
