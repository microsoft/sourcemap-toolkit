# Callstack Test App
This is a test app that hosts a webpage used for generating callstacks on various browsers. The webpage uses the windows.onerror event handler to report the information retrieved on the error object's stack property. If the browser does not return the error object on the windows.onerror event handler, it attempts to report the global window.events.error.stack property. By default the test app loads JavaScript minified by Ajaxmin (CrashCauser.js) which contains logic to generate a callstack when the "Cause Crash" button is clicked. This JavaScript is minified automatically as part of the msbuild process. There is also sample JavaScript checked in that has been minified with Google Closure (closureCrashCauser.js->closureCrashCauser.minified.js). In order to avoid the test app from taking a dependency on java, the process to minify the javascript and generate a sourcemap with closure is manual and if any changes are made to closurecrashcauser.js, the corresponding closurecrashcauser.minified.js and closurecrashcauser.sourcemap would need to be updated manually. This is done by running the locally on your machine and updated the minfied js and sourcemap. To switch the testapp to use the different minfiied versions of the JavaScript, you need to change the script tag loaded in index.html.

##JavaScript Minified using Ajax Min (CrashCauser.min.js)
CrashCauser.js is minified with Ajaxmin. The minfied version of the JavaScript and the sourcemap are generated at build time and not checked in via gitignore rules (crashcauser.min.js and crashcauser.min.js.map). The deminified stackframe should look similar to the following
```
level3
level3
level2
level1
causeCrash
window
```
## Javascript minified using Google Closure (ClosureCrashCauser.minified.js)
Closurecrashcauser.js is minified with Google Closure. The minified version of the javascript and the sourcemap are generated manually and are part of the source code (closurecrashcauser.minified.js and cclosurecrashcauser.sourcemap). The deminified stackframe should look similar to the following

```
mynamespace.objectWithMethods.propertyMethodLevel2
mynamespace.objectWithMethods.prototypeMethodLevel1
GlobalFunction
window
```
