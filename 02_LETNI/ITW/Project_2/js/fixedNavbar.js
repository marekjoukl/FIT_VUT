// Fixed navbar script
document.addEventListener("DOMContentLoaded", function () {
  const nav = document.querySelector("#nav");
  let navTop;

  function setNavTop() {
    navTop = nav.offsetTop;
    fixedNav(); // Apply fixed position immediately after calculating navTop
  }

  function fixedNav() {
    if (window.scrollY >= navTop) {
      nav.classList.add("fixed");
    } else {
      nav.classList.remove("fixed");
    }
  }

  window.addEventListener("scroll", fixedNav);

  setNavTop(); // Calculate navTop after page load
});
