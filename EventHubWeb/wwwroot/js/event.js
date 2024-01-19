$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#eventsData').DataTable({
        "language": {
            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/bg.json',
        },
        "ajax": { url: '/admin/event/getall' },
        "columns": [
            { data: 'name' },
            { data: 'place' },
            { data: 'description' },
            { data: 'category.name' },
            { data: 'price' },
            {
                data: 'dateAndTime',
                render: function (data, type, row) {
                    // Format the date into a human-readable format
                    var date = new Date(data);
                    return date.toLocaleString(); // Adjust the format as needed
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/event/edit?id=${data}" class="btn btn-primary mx-2" style="background-color: #94c045; border: #94c045;"> <i class="bi bi-pencil-square"></i></a>               
                     <a href="/admin/event/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i></a>
                    </div>`
                }
            }
        ]
    });
}