$(document).ready(function () {
    $("#createUser").click(function () {
        const registration = {
            Email: $("#cEmail").val(),
            Password: $("#cPass").val(),
            ConfirmPassword: $("#cConfPass").val()
        };
        $.post("api/Account/register",
            registration,
            function (data, status) {
                $("#createStatus").text("Status: " + status);
                $("#createData").text("Data: " + JSON.stringify(data));
                clearError();
            }
        );
    });

    $("#loginUser").click(function () {
        $.post("api/Account/login",
            {
                Email: $("#lEmail").val(),
                Password: $("#lPass").val()
            },
            function (data, status) {
                $("#loginStatus").text("Status: " + status);
                $("#loginData").text("Data: " + JSON.stringify(data));
                clearError();
            });
    });

    $(document).ajaxError(function (event, xhr, options, exc) {
        $("#errorStatus").text("Status: " + xhr.status + " " + xhr.statusText);
        $("#errorData").text("Data: " + JSON.stringify(xhr.responseJSON));
    });

    function clearError() {
        $("#errorStatus").text("");
        $("#errorData").text("");
    }
});