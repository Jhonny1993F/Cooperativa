function abrirModal(img) {
    let modal = document.getElementById("modalComprobante");
    let imgAmpliada = document.getElementById("imgAmpliada");
    imgAmpliada.src = img.src; // Asigna la imagen al modal
    modal.style.display = "flex"; // Muestra el modal
}

function cerrarModal() {
    document.getElementById("modalComprobante").style.display = "none";
}
