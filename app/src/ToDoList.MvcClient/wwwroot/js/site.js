// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

showPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (result) {
            $('#form-modal .modal-body').html(result);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        }
    })
}

ajaxPost = form => {
    try {
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (result) {
                $('#view-all').html(result);
                $('#form-modal .modal-body').html("");
                $('#form-modal .modal-title').html("");

                $('#form-modal').modal('hide');
            },
            error: function (err) {
                console.log(err);
            }
        });
    } catch (e) {
        console.log(e)
    }

    return false;
}

ajaxDelete = form => {
    if (confirm("Delete?")) {
        try {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (result) {
                    $("#view-all").html(result);
                },
                error: function (err) {
                    console.log(err);
                }
            });
        } catch (e) {
            console.log(e);
        }

        return false;
    }
}


ajaxChangeStatus = checkbox => {
    let self = $(checkbox);

    let id = self.attr("id");
    let isDone = self.prop("checked");

    try {
        $.ajax({
            type: "POST",
            url: self.data("url"),
            data: {
                id: id,
                isDone: isDone
            },
            success: function (result) {
                $("#view-all").html(result);
            },
            error: function (err) {
                console.log(err);
            }
        });
    } catch (e) {
        console.log(e);
    }

    return false;
}

