Name : Image2PDF Command Line v3.0
Price: $59
Requirements to system 
    64M or more memory  
    100 MHz processor 
Buy Online 
Free Download

 
--------------------------------------------------------------------------------
  
General 
    Image2PDF is a Windows application which can directly convert dozens of image formats, such as TIF, JPG, GIF, PNG, BMP, PSD, WMF, EMF, PDF, PCX, PIC and so on, into PDF format. Image2PDF can automatically clear and skew-correct B/W images by employing special techniques to insure high quality output after conversion. Image2PDF can operate independently of Adobe Acrobat and has faster processing speed. If your application doesn't require OCR (Optical Character Recognition) functionality, Image2PDF provides a very convenient, simple way to compose electronic books which can then be issued to the Web. You can scan paper documents directly to image files and then convert them to PDF using Image2PDF. 

    We also provide a command line version of Image2PDF (which allows you to manually issue commands or include them in scripts) as well as a CGI process which will allow, for example, a web site or intranet to convert TIFF or other image files to PDF on the fly. Image2PDF is perfect for high-volume document archive/database systems that require unattended batch image conversions (TIFF or other images) to PDF format. 


    
Image2PDF Command Line v3.0 Function and face 
Supports TIFF, JPG, PNG, GIF, PCD, PSD, TGA, BMP, DCX, PIC, EMF, WMF, etc. image formats 
Can combine multiple directories and images into one PDF file 
Does NOT need Adobe Acrobat software  
Supports Win98/ME/NT/2000/XP platforms  
Supports Multi-page image file to PDF conversion  
Supports sorting on file name when converting batches  
Supports single file processing, single directory processing, multi-level directory processing and many other processing modes  
Can set automatic despeckling (of B/W images; removes noise) and skew-correction  
When batch converting, several image files can be merged to create a single PDF file, or each image file may be converted into individual PDF files  
Makes use of effective compression processing to minimize space occupied by the created PDF files; for example, an A4 size file with a B/W image the output size is about 40KB to 50KB per page; color and gray-scale images are also compressed and optimized  
Specify any resolution in the generated PDF file  
Easily generate bookmarks to go to a specific page 
Supports command line operation (for manual use or inclusion in scripts) 
 
-------------------------------------------------------
Usage: Img2PDF [options] [-o output] [images]
-l [log file name] : specify log file for output message
-j [subject]       : subject
-t [title]         : title
-a [author]        : author
-k [keywords]      : keywords
-p [0 or 1]        : append to an exist pdf file, 0:insert at first page,1:append to last page
-s                 : skew correct
-c                 : clear spot
-r [resolution]    : set resolution in generated pdf file
   -r  0           : use the default image width and height information
   -r -1           : take over DPI info from original images
-o [pdf file name] : pdf file will be generated
-b [num]           : specify bookmark attribute
    num can use any one of the following values:
    >= 0           : specify first number in bookmarks
    == -1          : don't use bookmark
    == -2          : read bookmarks from bookmark.ini file
    == -3          : using the filenames as bookmarks
-------------------------------------------------------
Example:
     Img2PDF -b 10 -o c:\sample.pdf -r 100 c:\a1.tif c:\a2.jpg c:\a3_dir
     Img2PDF -b -3 -o c:\sample.pdf c:\*.tif
     Img2PDF -b -1 -o c:\sample.pdf -r 300 c:\a*.jpg
     Img2PDF -b -3 -o d:\*.pdf -r 300 c:\a*.jpg
     Img2PDF -b -3 -o "c:\pdf dir\*.pdf" "c:\*.*"
     Img2PDF -j "subject" -t "title" -a "author" -k "keywords" -o c:\sample.pdf c:\a1.tif
     Img2PDF -p 0 -o c:\sample.pdf c:\input_firstpage.tif
     Img2PDF -p 1 -o c:\sample.pdf c:\input_lastpage.tif
-------------------------------------------------------

 
Usage:
1.Convert one folder in to a pdf file:
Img2PDF.exe -o F:\output.pdf F:\inputdir

2.Convert multi folder into a pdf file:
Img2PDF.exe -o F:\output.pdf F:\inputdir1 F:\inputdir2

3.Convert a tiff file into a pdf file:
Img2PDF.exe -o F:\output.pdf F:\inputfile.tif

4.Convert tiff files and multi directory into a pdf file:
Img2PDF.exe -o F:\output.pdf F:\inputfile.tif F:\inputdir1 F:\inputdir2 
 

Limitation:
1.The trial version popup a message box, but you may click "No" button to continue evaluate it.
2.The trial version has ten pages limitation.

The registered version hasn't above limitations.


Support:
If you encounter any problems, please visit Image2PDF FAQ page for detail information,
http://www.globalpdf.com/tif2pdf/support/index.html
http://www.toppdf.com/tif2pdf/support/index.html
http://www.verypdf.com/tif2pdf/support/index.html

Contact us:
  mailto:support@verypdf.com
  http://www.verypdf.com/tif2pdf/tif2pdf.htm
 
 
