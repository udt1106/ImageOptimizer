# ImageOptimizer
Sitecore Image Optimizer, kamsar's Dianoga with File Size Restriction

The compression processor is from Kamsar's Dianoga (https://github.com/kamsar/Dianoga) which is automatic image optimizer only for PNG/JPG images. Even Dianoga is independent project, it was combined with ImageMaxSize validator. 

You can easily set restricted file extension and file size through "ImageOptimizer.config" file.

How to Install

1. Place "ImageCompressionTool" folder in "/WebSite/"
2. Place "ImageOptimizer.dll" in "/WebSite/bin/"
3. Place "ImageOptimizer.config" in "/WebSite/App_Config/Include/"
4. That's it.
  


How It Works

1. Upload Image File in Media Library
2. Check the file which is in restrictedExtension list
3. If the extension is in the list, check file size
4. If the file size is over than sizeValue, block the upload process


You can also download at Sitecore Market (https://marketplace.sitecore.net/Modules/Image_Optimizer)

Thank you, jammykam!!!
