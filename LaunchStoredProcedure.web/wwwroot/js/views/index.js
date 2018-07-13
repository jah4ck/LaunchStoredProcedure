$(document).ready(function () {

    



    $("#selectproc").change(function () {
        $("#content input").remove();
        $("#content p").remove();
        $("#result div").remove();

        $("#content").append('<input type="hidden" id="procname" name="procname" value="' + this.value + '">');
        

        $.ajax({
            url: $('#url_list_param').val(),
            type: 'GET',
            data: { id: this.value },
            dataType: 'html',
            success: function (result) {
                displayParam(result);
            }
        });
    });

});

function displayParam(result) {
    var obj = JSON.parse(result);
    if (obj.lstParamStoredProcedure.succeeded == true) {
        var retour = obj.lstParamStoredProcedure.result;
        var nbParam = retour.length;
        var i = 0;
        $.each(retour, function (i, val) {
            
            $("#content").append('<p> ' + val.parameteR_NAME + ' : ');
            $("#content").append('<input type="text" id="MyLstStoredProcedureParameters_' + i + '__valSaisie" name="MyLstStoredProcedureParameters[' + i + '].valSaisie" value="">');
            $("#content").append('<input type="hidden" id="MyLstStoredProcedureParameters_' + i + '__PARAMETER_NAME" name="MyLstStoredProcedureParameters[' + i + '].PARAMETER_NAME" value="' + val.parameteR_NAME+'">');
            $("#content").append('</p> ');
            i++;
        });

    }
}
function onBefore(e) {
    //faire un chargement dans un gif
    return true;
}

function onCompleteCalc(e) {
    alert("complete");
    var obj = JSON.parse(e.responseText);
    //var tab = obj.result.lstResultSet;//liste des resultset
    var tab = obj.lstResultSet;//liste des resultset
    var t = 0;
    var procedureName = $("#procname").val();

    var tableau = '';
    $.each(tab, function (index, item) {
        var i = 0;
        if (item.length > 0) {
            var colonneName = Object.keys(item[0]);
            tableau += '<div class="table-wrapper-2">';
            //tableau += '<button id = "test" onClick = "SaveResultCSV(' + t + ',\'' + procedureName + '\')" > Export csv</button >';
            tableau += '<a href="/Home/SaveResultCsv?id=' + t + '&amp;sp_name=' + procedureName+'">Export csv</a>';
            tableau += '<table id = "tableresult" class="table table-striped table-hover table-bordered" > ';
            tableau += '<thead class="thead-dark"><tr>';
            $.each(colonneName, function (index, col) {
                tableau += '<th scope="col">' + col + '</th>';
            });
            tableau += '</tr></thead><tbody>'
            $.each(item, function (index, item) {
                i++;
                tableau += '<tr>';
                $.each(item, function (index, item) {
                    tableau += '<td scope="col">' + item + '</td>';
                });
                tableau += '</tr>';
                if (i === 100) {
                    return false;
                }
            });
            tableau += '</tbody>'
            tableau += '</table></div>';
            t++;
        }
    });
    $("#result").append(tableau);
}

//function SaveResultCSV(idtab, spname) {
//    $.ajax({
//        url: $('#url_saveResultCsv').val(),//appel de la method get AdresseTableau
//        type: 'GET',
//        data: {
//            id: idtab,
//            sp_name: spname
//        },
//        //dataType: 'html',
//        success: function (result) {
//            alert(result);
//        }
//    });
//}