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
            "url": "/api/products/datatable",
            "dataSrc": "aaData"
        },
        "columns": [
            { "data": "title", "width": "20%" },
            { "data": "author", "width": "20%" },
            { "data": "isbn", "width": "20%" },
            { "data": "category.name", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Product/Upsert/${data}" class="btn btn-success text-white" style="cursor: pointer">
                                    <i class="fa fa-edit"></i>
                                </a>
                                <a onclick=Delete("/api/products/${data}") class="btn btn-danger text-white" style="cursor: pointer">
                                    <i class="fa fa-trash"></i>
                                </a>
                            </div>
                           `;
                },
                "width": "20%"
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
                    toastr.success("Product deleted successfully");
                    dataTable.ajax.reload();
                },
                error: function (data) {
                    toastr.error("Could not delete category: " + data);
                }
            });
        }
    });
}
