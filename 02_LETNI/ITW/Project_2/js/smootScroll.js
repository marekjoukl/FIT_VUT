// Smooth scroll script
let navlinks = document.querySelectorAll("#sidemenu li:not(:last-child) a");
for (const link of navlinks) {
  link.addEventListener("click", smoothScroll);
}

function smoothScroll(event) {
  event.preventDefault();

  const href = this.getAttribute("href");

  // Check if the href attribute starts with '#'
  if (href.startsWith("#")) {
    const targetElement = document.querySelector(href);

    if (targetElement) {
      const yOffset = -80;
      const y =
        targetElement.getBoundingClientRect().top + window.scrollY + yOffset;

      window.scrollTo({ top: y, behavior: "smooth" });
    }
  } else {
    // Handle external URL navigation
    window.location.href = href;
  }
}
