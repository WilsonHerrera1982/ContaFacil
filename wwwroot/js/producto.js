$(document).ready(function () {
    var table = $('#tblProducto').DataTable({
        dom: 'Bfrtip',
        fixedHeader: true,
        responsive: true,
        pageLength: 25,
        columnDefs: [
            { targets: -1, visible: false } // Oculta la última columna (opciones) en la vista de tabla
        ],
        language: {
            processing: "Procesando...",
            search: "Buscar:",
            lengthMenu: "Mostrar _MENU_ entradas",
            info: "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            infoEmpty: "Mostrando 0 a 0 de 0 entradas",
            infoFiltered: "(filtrado de _MAX_ entradas totales)",
            infoPostFix: "",
            loadingRecords: "Cargando...",
            zeroRecords: "No se encontraron registros.",
            emptyTable: "No hay datos disponibles en la tabla.",
            paginate: {
                first: "Primero",
                previous: "Anterior",
                next: "Siguiente",
                last: "Último"
            },
            aria: {
                sortAscending: ": activar para ordenar la columna en orden ascendente",
                sortDescending: ": activar para ordenar la columna en orden descendente"
            }
        },
        buttons: [
           /* {
                extend: 'excelHtml5',
                exportOptions: {
                    columns: ':visible:not(:last-child)' // Excluye la última columna (opciones)
                }
            },*/
            {
                extend: 'pdfHtml5',
                exportOptions: {
                    columns: ':visible:not(:last-child)' // Excluye la última columna (opciones)
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: ':visible:not(:last-child)' // Excluye la última columna (opciones)
                }
            }
        ],
        headerCallback: function (thead, data, start, end, display) {
            $(thead).find('th').attr('scope', 'col');
        }
    });
});
