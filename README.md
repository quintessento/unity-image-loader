# unity-image-loader
 Simple image loader, made in the Unity engine. Loads a list of images from a folder and displays their previews and information.
 
 Features:
 
- File browser that let's you choose a folder on your system;
- Input for extensions. Use * for all extensions;
- Refresh button to initiate loading;
- Image loader list that displays images with previews, file names, and creation dates;
- Coroutines are used to load images in a faked asynchronous way -- list items will appear fast and image previews will show up as they are loaded;
- While images are loading, progress can be seen in the form of a spinner. While it isn't accurate, it can be faked by enabling the corresponding option on the Image Loader scene object, which will let the spinners complete a full circle, while still dependent on the loading time.
