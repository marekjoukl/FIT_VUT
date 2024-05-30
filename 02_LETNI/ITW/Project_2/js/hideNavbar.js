document.addEventListener("DOMContentLoaded", function () {
  const navbar = document.getElementById("nav");
  const sidemenu = document.getElementById("sidemenu");
  let prevScrollPos = window.scrollY;

  function handleScroll() {
    let currentScrollPos = window.scrollY;
    if (prevScrollPos > currentScrollPos) {
      navbar.style.top = "0";
    } else {
      if (sidemenu.style.right != "0px") navbar.style.top = "-70px";
    }
    prevScrollPos = currentScrollPos;
  }

  if (
    window.matchMedia("(min-width: 769px) and (max-width: 1024px)").matches ||
    window.matchMedia("(max-width: 768px)").matches
  ) {
    window.addEventListener("scroll", handleScroll);
  } else {
    window.removeEventListener("scroll", handleScroll);
  }
});
