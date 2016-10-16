(function (window, document) {

    var menu     = document.getElementById('menu'),
        menuToggle = document.getElementById('menuToggle');
    var exitBtn =  document.getElementById('exit');

    function toggleClass(element, className) {
        var classes = element.className.split(/\s+/),
            length = classes.length,
            i = 0;

        for(; i < length; i++) {
          if (classes[i] === className) {
            classes.splice(i, 1);
            break;
          }
        }
        // The className is not found
        if (length === classes.length) {
            classes.push(className);
        }

        element.className = classes.join(' ');
    }

    menuToggle.onclick = function (e) {
        var active = 'active';

        e.preventDefault();
        toggleClass(menu, active);
        toggleClass(menuToggle, active);
    };

    exitBtn.onclick = function (e) {
     if(window.external.Close) {
       window.external.Close();
     }else {
       open(location, '_self').close();
     }
   };

   oncontextmenu="return false"

}(this, this.document));
