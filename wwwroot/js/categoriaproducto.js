$(document).ready(function () {

    var table = $('#tblCategoriaProducto').DataTable({

        dom: 'Bfrtip',
        fixedHeader: true,
        responsive: true,
        pageLength: 25,
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

            {
                extend: 'excelHtml5',
                customize: function (xlsx) {

                    // var total = $('#total').text();
                    // Agregar fila personalizada
                    var sheet = xlsx.xl.worksheets['sheet1.xml'];
                    var lastRow = $('row:last', sheet);
                    lastRow.after(newRow);


                }
            },
            {
                extend: 'pdfHtml5',
                footer: true,
                customize: function (doc) {

                    // Obtener valor del label                    
                    var fecha = $('#fec').text();
                    // Agregar fila personalizada
                    // doc.content.splice(0, 0, { text: fecha, style: 'header', alignment: 'center', bold: true });

                    // Agregar fila personalizada al final del documento                    
                    doc.content.push({ text: '\n', style: 'header', alignment: 'center', bold: true });
                    doc.content.push({ text: '\n', style: 'header', alignment: 'center', bold: true });
                    doc.content.push({ text: '\n', style: 'header', alignment: 'center', bold: true });
                    doc.content.push({ text: '\n', style: 'header', alignment: 'center', bold: true });
                    doc.content.push({ text: fecha, style: 'header', alignment: 'rigth', bold: true });
                }
            },
            {
                extend: 'print',
                customize: function (win) {
                    // Obtener valor del label
                    var empresa = $('#emp').text();
                    var sub = $('#sub').text();
                    var fecha = $('#fec').text();
                    var total = $('#total').text();
                    var credito = $('#credito').text();
                    // Agregar fila personalizada
                    $(win.document.body).prepend('<h2 style="text-align: center; font-weight: bold;">' + fecha + '</h2>');
                    $(win.document.body).prepend('<h2 style="text-align: center; font-weight: bold;">' + sub + '</h2>');
                    $(win.document.body).prepend('<h2 style="text-align: center; font-weight: bold;">' + empresa + '</h2>');
                    $(win.document.body).append('<h2 style="text-align: right; font-weight: bold;">Total: ' + total + '</h2>');
                    $(win.document.body).append('<h2 style="text-align: right; font-weight: bold;">Credito: ' + credito + '</h2>');
                }
            }
        ],
        headerCallback: function (thead, data, start, end, display) {
            $(thead).find('th').attr('scope', 'col');
        },

    });
});
