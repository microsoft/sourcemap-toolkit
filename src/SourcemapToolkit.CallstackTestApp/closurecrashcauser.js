var mynamespace = {};

mynamespace.objectWithMethods = function () { };
window["foo"] = mynamespace.objectWithMethods;
mynamespace.objectWithMethods.prototype = { prototypeMethodLevel1: function () { var x; return mynamespace.objectWithMethods.propertyMethodLevel2(x); } }

mynamespace.objectWithMethods.propertyMethodLevel2 = function (e) { return e.length; }

function GlobalFunction() { var x = new mynamespace.objectWithMethods(); return x.prototypeMethodLevel1(); }

window["foo"]["bar"] = mynamespace.objectWithMethods.prototypeMethodLevel1;
window["foo"]["bar2"] = mynamespace.objectWithMethods.propertyMethodLevel2;
window["bar"] = GlobalFunction;

window.onerror = function (message, source, lineno, colno, error) {
    document.getElementById("callstackdisplay").innerText = error.stack;
}

window.onload = function (event) {
    document.getElementById("crashbutton").addEventListener("click", function () {
        console.log(GlobalFunction());
    });
}