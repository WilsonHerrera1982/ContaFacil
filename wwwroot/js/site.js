// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Agregar esto al archivo site.js o crear un nuevo archivo JavaScript

$(document).ready(function () {
    // Interceptar todas las solicitudes AJAX
    $(document).ajaxStart(function () {
        showLoading();
    });

    $(document).ajaxStop(function () {
        hideLoading();
    });

    // Interceptar envíos de formularios
    $('form').submit(function () {
        showLoading();
    });

    // Interceptar clics en enlaces que no sean AJAX
    $('a:not(.ajax-link)').click(function () {
        showLoading();
    });
});

function showLoading() {
    $('#loading-overlay').show();
}

function hideLoading() {
    $('#loading-overlay').hide();
}

// Opcionalmente, puedes agregar un temporizador para ocultar el loading después de un tiempo máximo
var loadingTimer;

function showLoadingWithTimeout() {
    showLoading();
    clearTimeout(loadingTimer);
    loadingTimer = setTimeout(hideLoading, 30000); // 30 segundos de tiempo máximo
}

// Interceptar la navegación del navegador
$(window).on('beforeunload', function () {
    showLoading();
});
$(document).ajaxComplete(function (event, xhr, settings) {
    if (xhr.getResponseHeader('X-Loading-Complete') === 'true') {
        hideLoading();
    }
});