function showSection(sectionId, element) {

    // 🔥 đổi section
    const sections = document.querySelectorAll('.section');
    sections.forEach(sec => sec.classList.remove('active'));

    document.getElementById(sectionId).classList.add('active');

    // 🔥 đổi menu active
    const menuItems = document.querySelectorAll('.menu-item');
    menuItems.forEach(item => item.classList.remove('active'));

    element.classList.add('active');

    function showSection(sectionId) {
        const sections = document.querySelectorAll('.section');
        sections.forEach(sec => sec.classList.remove('active'));

        document.getElementById(sectionId).classList.add('active');
    }
}
