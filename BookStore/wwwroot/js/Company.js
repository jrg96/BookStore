var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        'processing': true,
        'paging': true,
        'serverSide': true,
        'serverMethod': 'get',
        "ajax": {
            "url": "/api/companies/datatable",
            "dataSrc": "aaData"
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Company/Upsert/${data}" class="btn btn-success text-white" style="cursor: pointer">
                                    <i class="fa fa-edit"></i>
                                </a>
                                <a onclick=Delete("/api/companies/${data}") class="btn btn-danger text-white" style="cursor: pointer">
                                    <i class="fa fa-trash"></i>
                                </a>
                            </div>
                           `;
                },
                "width": "40%"
            }
        ]
    });

}


function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    toastr.success("Company deleted successfully");
                    dataTable.ajax.reload();
                },
                error: function (data) {
                    toastr.error("Could not delete company: " + data);
                }
            });
        }
    });
}