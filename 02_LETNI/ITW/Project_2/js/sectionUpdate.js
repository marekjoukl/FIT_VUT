function getCurrentSection() {
  const sections = document.querySelectorAll("section");

  let currentSection = null;
  let minDistance = Infinity;

  sections.forEach((section) => {
    const distance = Math.abs(section.getBoundingClientRect().top + 300);
    if (distance < minDistance) {
      minDistance = distance;
      currentSection = section;
    }
  });

  return currentSection;
}

function updateActiveLink() {
  const currentSection = getCurrentSection();
  const navLinks = document.querySelectorAll("#nav a[href^='#']");

  navLinks.forEach((link) => {
    const href = link.getAttribute("href").substring(1);

    if (currentSection && currentSection.id === href) {
      link.classList.add("active");
    } else {
      link.classList.remove("active");
    }
  });
}

window.addEventListener("scroll", updateActiveLink);

// Initial update on page load
document.addEventListener("DOMContentLoaded", function () {
  // Call updateActiveLink() once the DOM is fully loaded
  updateActiveLink();
});
