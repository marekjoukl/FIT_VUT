// Sidebar script
let sidemenu;

document.addEventListener("DOMContentLoaded", function () {
  sidemenu = document.getElementById("sidemenu");

  // Add event listener to close the sidebar when clicking anywhere on the document
  document.addEventListener("click", function (event) {
    // Check if the clicked element is inside the sidebar
    if (
      !sidemenu.contains(event.target) &&
      sidemenu.style.right == "0px" &&
      event.target.id != "burger-menu"
    ) {
      closemenu();
    }
  });
});

function openmenu() {
  sidemenu.style.right = "0";
}
function closemenu() {
  sidemenu.style.right = "-240px";
  event.preventDefault();
}
