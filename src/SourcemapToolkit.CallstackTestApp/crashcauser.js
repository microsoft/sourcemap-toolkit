function causeCrash()
{
    function level1() {
        var longLocalVariableName = 16;
        longLocalVariableName += 2;
        level2(longLocalVariableName);
    }

    function level2(input) {
        input = input + 2;
        level3(input);
    }

    function level3(input) {
        (function() {
            var x;
            console.log(x.length + input);
        }());
    }

    window.onerror = function (message, source, lineno, colno, error) {
        if (error)
            document.getElementById("callstackdisplay").innerText = error.stack;
        else if (window.event.error)
            document.getElementById("callstackdisplay").innerText = window.event.error.stack;
    }

    level1();
}

window.onload = function (event) {
    document.getElementById("crashbutton").addEventListener("click", function () {
        causeCrash();
    });
}