$(document).ready(function () {
    function Contains(text_one, text_two) {
        if (text_one.indexOf(text_two) != -1)
            return true;
    }

    $("#Search").keyup(function () {
        var searchText = $("#Search").val().toLowerCase();
        $(".fila").each(function () {
            if (!Contains($(this).text().toLowerCase(), searchText)) {
                $(this).hide();
            }
            else {
                $(this).show();
            }
        });
    });
    var check = document.getElementById('id_check');
    $("#sem").hide();
    check.addEventListener('click', function () {
        if (check.checked) {
            
            $("#sem").show();
            console.log($('sem').val());
        } else {
            $("#sem").hide();
            console.log($('sem').val());
        }
    })
    all_masterlist();
    
   // all_users();

    Get_Devices();
    Get_departaments();

    
    
  //  $("#div_details").hide();
    $("#id_description_equipment_0").change(function () {
        Get_TypeEquipment();
        $("#idcode_e").empty();
        $("#brad_0").empty();
        $("#brad_1").empty();
        $("#model_0").empty();
        $("#model_1").empty();
        $("#id_description_equipment_1").empty();
        Get_details();
        Get_brand();
        $("#buyer").empty();
        Get_departaments();
    })

    



   
});
/*-------------------------------------LIST MASTER-----------------------------------------------*/
function all_masterlist() {
    $.ajax({

        url: "/siixsem_it/GetMaster_List",
        //data: JSON.stringify(empObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success:
            function (result) {
                var html = '';
                $.each(result, function (key, item) {

                    html += '<tr class="fila row_table" id="fila">';
                    html += '<td><center>' + item.e_name + '-' + item.id_eq + '</center></td>';
                    html += '<td><center>' + item.brand + '</center></td>';
                    html += '<td><center>' + item.series + '</center></td>';
                    html += '<td><center>' + item.model_name + '</center></td>';
                    html += '<td><center>' + item.name_dep + '</center></td>';
                    html += '<td><center>' + item.ubication + '</center></td>';
                    html += '<td><center>' + item.e_status + '</center></td>';
                    //html += '<td><center>' + item.warranty + '</center></td>';
                    html += '<td><center>' + item.cod_description + '</center></td>';
                    //html += '<td><center>' + item.name_procesador + '</center></td>';
                    //html += '<td><center>' + item.characteristic1 + '</center></td>';
                    //html += '<td><center>' + item.characteristic2 + '</center></td>';
                    //html += '<td><center>' + item.characteristic3 + '</center></td>';
                    html += '<td><center>' + item.comments + '</center></td>';
                    html += '<td><button type="button" class="btn btn-default  fa fa-pencil" data-toggle="modal" data-target="#edit_equipment" style="background:#ffde59; " onclick="return getbyID_Details_E(' + item.id + ')"></button></td>';
                    html += '<td><button type="button" class="btn btn-default  fa fa-bars" data-toggle="modal" data-target="#hist_equipment" style="background:#FF738E; " onclick="return getbyHistoryc_Equip(' + item.id + ')"></button></td>';
                    //html += '<td><center>' + item.name_file + '</center></td>';
                    //html += '<td>' +
                    //    '<form action="Dowland_Responsive" method="post">' +
                    //    '<input value="' + item.name_file + '" type="hidden" name="name_file" />' +
                    //    '<center><button class="fa  fa-download btn-sm btn btn-default " style="background:#5CE1E6;"   type="submit"></button></center>' +
                    //    '</form>' +
                    //    '</td>';
                    //html += '<td><center><input type="submit" value="Enviar" /></center></td>';
                    //html += '<td><center><button style="background:#FFDE59;" class="btn btn-sm btn-default mt-2 " data-toggle="modal" data-target="#edit_equipment" data-id="' + item.id + '"><i class="fas fa-bars"></i></button></center></td>';
                    //html += "<td><button class='btn btn-sm btn-primary mt-2' data-toggle='modal' data-target='#edit-employee-modal' data-id='"+value.id+"'>Edit</button></td>";
                    // html += '<td><button type="button" class="btn btn-default  fa fa-pencil" data-toggle="modal" data-target="#myModal" style="background:#ffde59; " onclick="return get(' + item.id_employe + ')"></button></td>';

                   // html += '<td><button type="button" class="btn btn-default  fa fa-pencil" data-toggle="modal" data-target="#myModal" style="background:#ffde59; " onclick="return getbyID(' + item.id_ticket + ')"></button></td>';

                    html += '</tr>';});
                $('.tbody').html(html);
                }
            });
}
function Add_Equipment() {

    var input_sem = $('#sem').val();
    var warranty = $('#warranty').val();
    var bitlocker = $('#bitlocker').val();
    var activo_fijo = $('#fixed_active').val();
    var comments = $('#comments').val();
    var iddep = $('#buyer').val();
    //var iddetails = $('#procesador').val();
    var ubicacion = $('#ubicacion').val();
    var status = $('#status').val();
    var id_procesador = $('#procesador').val();
    var ram = $('#ram').val();
    var type_disk = $('#type_disk').val(); 
    var almacenamiento = $('#almacenamiento').val(); 

    var yea = document.getElementById("table_body").rows.length;
    //console.log($('sem').val());
    if (yea <= 2) {
        if (series != "") {
            for (j = 0; j < yea; j++) {
                var id_description_equipment = $('#id_description_equipment_' + j).val();
                console.log("tIPO DE DISSITIVO"+id_description_equipment);
                var series = $('#series_' + j).val();
                console.log(series);
                var brand = $('#brad_'+j).val();
                var model_name = $('#model_' + j).val();
                console.log(model_name);
                console.log(j);
            $.ajax({
                url: "/siixsem_it/add_equipment_it",
                type: "POST",
                data: {
                    id_code_e: id_description_equipment,
                    series: series,
                    brand: brand,
                    warranty: warranty,
                    model_name: model_name,
                    iddep: iddep,
                    comments: comments,
                    activo_fijo: activo_fijo,
                    bitlocker: bitlocker,
                    //iddetails: iddetails,
                    e_status: status,
                    sem: input_sem,
                    ubication: ubicacion,
                    id_procesador: id_procesador,
                    id_ram_memory: ram,
                    id_type_disk: type_disk,
                    disk_capacity: almacenamiento
                },
                cache: false,
                beforeSend:
                    function (cargando) {
                        //$("#loader").show();
                        //document.getElementById("btnAdd").disabled = true;
                    },
                success: function (result) {
                    //$("#loader").hide();
                    alertify.notify('¡Se añadio dispositivo al inventario con éxito!', 'success', 1, function () { location.reload(); });
                  
                },

                error: function (errormessage) {
                    alertify.error('Upss... Ocurrio un error comunicate al departamento de IT ¡Gracias!');
                    //document.getElementById("btnAdd").disabled = false;
                }
            });
            }
            
        }
        else {
            alert('Falta informaci{on por llenar!');
        }
    } else { alertify.error('Upss... No es posible añadir mas de 2 registros, se reiniciara página!',1, function () { location.reload();}); }
    
}
function Get_TypeEquipment(){

    var id_description_equipment = $('#id_description_equipment_0').val();
        $.ajax({
            url: "/siixsem_it/Get_typeEqui",
            type: "POST",
            data: {
                id: id_description_equipment
            },
            cache: false,
            success: function (result) {
                var table = document.getElementById("table_body");
                var totalRowCount = table.rows.length;
                switch (result) {
                    case "LP":
                        newline();
                         $("#div_details").show(); 
                        break;
                    case "WS":
                        newline();
                        $("#div_details").show();
                        break;
                    default:
                        
                        if (totalRowCount > 1) {
                            document.getElementById("table_body").deleteRow(1);
                            console.log(totalRowCount);
                            Get_details();
                            Get_brand();
                            $("#div_details").hide(); 
                        }
                       
                        break;
                }
            },

            error: function (errormessage) {
                alertify.error('Upss... Ocurrio un error comunicate al departamento de IT ¡Gracias!');
            }
        });
   
}
function newline() {
    var yea = document.getElementById("table_add").rows.length;
    var DATOS = { Contador: yea };
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: "{conta:" + JSON.stringify(DATOS) + "}",
        url: "/siixsem_it/newline2",
        success: function (data) {
            //alert( document.getElementsByClassName("ctn").length);
            $("#table_add").append(data);
            Get_Devices_Cargadores();
        },
        error: function (result) {
            alert("error");
        }
    });
}
/*---------------------Obtiene todos los dipos de equipos que se encuentran registrados----------------------------------------*/ 
function Get_Devices() {
    $.ajax({
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        url: "/siixsem_it/Get_Devices",
        success: function (data) {
            
            $("#id_description_equipment").empty();
            
            for (u in data) {
                $("#id_description_equipment_0").append("<option value=" + data[u].id_code_e + ">" + data[u].code_description + "</option>");
            }
            if ($('#id_description_equipment_0').val() == '1' || $('#id_description_equipment_0').val() == "3") { $(".div_details").show(); } else { $(".div_details").hide();}
        },
        error: function (result) {
            alert("error Articulos");
        }
    })
}
/*---------------------Obtiene todos los dipos de equipos que se encuentran registrados----------------------------------------*/
function Get_Devices_Cargadores() {
    var id_description_equipment = $('#id_description_equipment_0').val();
    $.ajax({
        url: "/siixsem_it/Get_Device_Car",
        type: "POST",
        data: {
            id_code_e: id_description_equipment
        },
        cache: false,
        beforeSend:
            function (cargando) {
                //$("#loader").show();
                //document.getElementById("btnAdd").disabled = true;
            },
        success: function (result) {
            for (u in result) {
                $("#id_description_equipment_1").append("<option value=" + result[u].id + ">Cargador</option>");
            }
            Get_brand_Carg();
        },

        error: function (errormessage) {
            alertify.error('Upss... Ocurrio un error comunicate al departamento de IT ¡Gracias!');
            //document.getElementById("btnAdd").disabled = false;
        }
    });
}

function Get_details() {
    var DATOS = { id_code_e: $('#id_description_equipment_0').val() }
   // console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
         data:  JSON.stringify(DATOS) ,
        url: "/siixsem_it/List_Details",
        /*success: function (data) {
           // $("#idcode_e").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#idcode_e").append("<option value=" + data[u].iddetails + ">" + data[u].characteristic2 + "</option>");
                
            }
            $("#idcode_e").change(function () {
               // $("#model").empty();
                Get_details_equipment();
                //if ($('#id_description_equipment').val() == '1' || $('#id_description_equipment').val() == "3") { $("#div_details").show(); } else { $("#div_details").hide(); }
            });
        },*/
        success: function (data) {

            $("#procesador").empty();
            
            $("#procesador").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#procesador").append("<option value=" + data[u].id_procesador + ">" + data[u].name_procesador + "</option>");
            }
            $("#procesador").change(function () {
                // $("#model").empty();
                Get_details_equipment();
                //if ($('#id_description_equipment').val() == '1' || $('#id_description_equipment').val() == "3") { $("#div_details").show(); } else { $("#div_details").hide(); }
            });
            
        },
        error: function (result) {
            alert(result);
        }
    });
}
/*-------------------------------Obtiene marcas del dispositivo seleccionado--------------------------------------------*/
function Get_brand() {
    var DATOS = { id_code_e: $('#id_description_equipment_0').val() }
    // console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/Get_Brands",
        success: function (data) {
           // $("#idcode_e").append("<option value=0>Selecciona Opción</option>");
            $("#brad_0").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#brad_0").append("<option value=" + data[u].id_code_e + ">" + data[u].brand + "</option>");

            }
            Get_models();
            
        },
        error: function (result) {
            alert(result);
        }
    });
}
/*-------------------------------Obtiene marcas del cargador seleccionado--------------------------------------------*/
function Get_brand_Carg() {
    var DATOS = { id_code_e: $('#id_description_equipment_1').val() }
    $.ajax({
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/Get_Brands",
        success: function (data) {
            $("#brad_1").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#brad_1").append("<option value=" + data[u].id_code_e + ">" + data[u].brand + "</option>");

            }
            Get_models_Car($('#id_description_equipment_1').val());

        },
        error: function (result) {
            alert(result);
        }
    });
}


function Get_models() {
    $("#brad_0").change(function () {
        $("#model_0").empty();
        var DATOS = { id_code_e: $('#id_description_equipment_0').val(), brand: $('#brad_0').val() }
        // console.log(DATOS);
        $.ajax({
            //data: { id_description_equipment: id_description_equipment },
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=uft-8",
            data: JSON.stringify(DATOS),
            url: "/siixsem_it/Get_Models",
            success: function (data) {
                $("#model_0").append("<option value=0>Selecciona Opción</option>");
                for (u in data) {
                    $("#model_0").append("<option value=" + data[u].id + ">" + data[u].model_name + "</option>");

                }
            },
            error: function (result) {
                alert(result);
            }
        });
        
    })
    
}


function Get_models_Car(id) {
    var DATOS = { id_code_e: id }
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/Get_Models",
        success: function (data) {
            $("#model_1").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#model_1").append("<option value=" + data[u].id + ">" + data[u].model_name + "</option>");
            }

        },
        error: function (result) {
            alert(result);
        }
    });
}

function Get_details_equipment() {
    Get_ram_memory();
    Get_type_disk();
    Get_disk_capacity();
  /*
    var DATOS = { id_code_e: $('#idcode_e').val() }
    console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/List_DetailsEquipment",
        success: function (data) {
            $("#procesador").empty();
            $("#procesador").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#procesador").append("<option value=" + data[u].id_procesador + ">" + data[u].name_procesador + "</option>");
            }
            Get_ram_memory();
            Get_type_disk();
            Get_disk_capacity();
        },
        error: function (result) {
            alert(result);
        }
    });*/
}
function Get_ram_memory() {

    var DATOS = { id_code_e: $('#idcode_e').val() }
    console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/List_ram_memory",
        success: function (data) {
            $("#ram").empty();
            $("#ram").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#ram").append("<option value=" + data[u].id_ram_memory + ">" + data[u].ram_memoria + "</option>");

            }
        },
        error: function (result) {
            alert(result);
        }
    });
}
function Get_type_disk() {

    var DATOS = { id_code_e: $('#idcode_e').val() }
    console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/List_type_disk",
        success: function (data) {
            $("#type_disk").empty();
            $("#type_disk").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#type_disk").append("<option value=" + data[u].id_type_disk + ">" + data[u].name_type_disk + "</option>");
                
            }
        },
        error: function (result) {
            alert(result);
        }
    });
}
function Get_disk_capacity() {

    var DATOS = { id_code_e: $('#idcode_e').val() }
    console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
        data: JSON.stringify(DATOS),
        url: "/siixsem_it/List_disk_memory",
        success: function (data) {
            $("#almacenamiento").empty();
            $("#almacenamiento").append("<option value=0>Selecciona Opción</option>");
            for (u in data) {
                $("#almacenamiento").append("<option value=" + data[u].id + ">" + data[u].disk_capacity + "</option>");
            }
        },
        error: function (result) {
            alert(result);
        }
    });
}

function Get_departaments() {
    // console.log(DATOS);
    $.ajax({
        //data: { id_description_equipment: id_description_equipment },
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=uft-8",
       
        url: "/siixsem_it/List_Departamens",
        success: function (data) {
            for (u in data) {
                $("#buyer").append("<option value=" + data[u].iddep + ">" + data[u].name_dep + "</option>");

            }
            
        },
        error: function (result) {
            alert(result);
        }
    });
}
//Function for getting the Data Based upon Employee ID
function getbyID_Details_E(id) {
    console.log(id);
    $.ajax({
        url: "/siixsem_it/Get_DetailsE/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success:
            function (result) {
                var html = '';
                var cont = 0;
                $.each(result, function (key, item) {
                    
                    html += '<center>' +
                        '<div class="row">' +
                        '<div class="col-sm-3">' +
                        '<h6>Nombre Equipo</h6>' +
                        '<input type="hidden" id="idModal" class="form-control" value="' + item.id + '" readonly />' +
                        '<input type="text" id="id_eq" class="form-control" value="' + item.e_name + "-" + item.id_eq + '" readonly />' +
                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Serial</h6>' +
                        '<input type="text" id="series" class="form-control" value="' + item.series + '"  readonly/>' +
                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Estatus</h6>' +
                        '<select id="statusM" class="form-control">' +
                            '<option value = "' + item.e_status +'" >'+item.e_status+'</option >' +
                            '<option value="Entregado" > Entregado</option >'+
                            '<option value="En preparación">En preparación</option>'+
                            '<option value="Resguardo IT">Resguardo IT</option>'+
                            '<option value="Asignado">Asignado</option>' +
                            '<option value="Disponible">Disponible</option>' +
                            '<option value="Obsoleto">Obsoleto</option>' +
                        '</select >';
                   
                    html += '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Ubicación</h6>' +
                        '<select id="ubicacionM" class="form-control">' +
                        '<option value = "' + item.ubication + '" >' + item.ubication + '</option >' +
                        '<option value = "Almacen IT" > Almacen TI</option >' +
                        '<option value="Con usuario">Con usuario</option>' +
                        '<option value="Oficina IT">Oficina IT</option>' +
                        '<option value="Site 1">Site 1</option>' +
                        '</select >' +
                        '</div>' +
                        '</div>' +
                        '<br><h6>Caracteristicas del equipo</h6>' +
                        '<div class="row">' +
                        '<div class="col-sm-3">' +
                        '<h6>Marca</h6>' +
                        '<select id="brad" name="brad" class="form-control" disabled>' +
                        '<option value="' + item.brand + '">' + item.brand + '</option>' +
                        '</select >' +
                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Modelo</h6>' +
                        '<select id="brad" name="brad" class="form-control" disabled>' +
                        '<option value="' + item.model_name + '">' + item.model_name + '</option>' +
                        '</select >' + '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Tipo de dispositivo</h6>' +
                        '<select id="cod_descriptionM" class="form-control" disabled>' +
                        '<option value = "' + item.id_eq + '" >' + item.cod_description + '</option >' +
                        '</select>'+

                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Termino de garantia</h6>' +
                        '<input type="datetime" id="warranty" class="form-control" value="' + item.warranty + '"  readonly/>' +
                        '</div>' +
                        '</div>' +
                        '<br>' +
                        '<div class="row">' +
                        '<div class="col-sm-3">' +
                        '<h6>Procesador</h6>' +

                        '<select id="id_procesadorM" class="form-control">' +
                        '<option value = "' + item.id_procesador + '" >' + item.name_procesador + '</option >';
                        switch (item.id_procesador) {
                                case ("1"):
                                html += '<option value = "2" >Core i3</option >'+
                                    '<option value = "3" >Core i5</option >'+
                                    '<option value = "4" >Core i7</option >';
                                break;
                            case ("2"):
                                html += '<option value = "1" >N/A</option >'+
                                    '<option value = "3" >Core i5</option >'+
                                    '<option value = "4" >Core i7</option >';
                                break;
                            case ("3"):
                                html += '<option value = "1" >N/A</option >'+
                                    '<option value = "2" >Core i3</option >'+
                                    '<option value = "4" >Core i7</option >';
                                break;
                            case ("4"):
                                html += '<option value = "1" >N/A</option >'+
                                    '<option value = "2" >Core i3</option >'+
                                    '<option value = "4" >Core i5</option >';
                                break;
                            default:
                                html += '<option value = "1" >N/A</option >'+
                                    '<option value = "2" >Core i3</option >'+
                                    '<option value = "3" >Core i5</option >'+
                                    '<option value = "4" >Core i5</option >';
                                break;
                        }
                            
                    html += '</select >' +

                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Almacenamiento</h6>' +
                        
                        '<select id="almacenamientooM" class="form-control">' +
                        '<option value = "' + item.id_almacenamiento + '" >' + item.disk_capacity + '</option >';
                    switch (item.disk_capacity) {
                            case ("N/A"):
                                html += '<option value = "2" >256 GB</option >' +
                                    '<option value = "3" >512 GB</option >' +
                                    '<option value = "4" >1 TB</option >';
                                break;
                        case ("256 GB"):
                                html += '<option value = "1" >N/A</option >' +
                                        '<option value = "3" >512 GB</option >' +
                                        '<option value = "4" >1 TB</option >';
                                break;
                        case ("512 GB"):
                                html += '<option value = "1" >N/A</option >' +
                                    '<option value = "2" >256 GB</option >' +
                                    '<option value = "4" >1 TB</option >';
                                break;
                        case ("1 TB"):
                            html += '<option value = "1" >N/A</option >' +
                                '<option value = "2" >256 GB</option >' +
                                '<option value = "3" >512 GB</option >';
                                break;
                            default:
                            html += '<option value = "1" >N/A</option >' +
                                '<option value = "2" >256 GB</option >' +
                                '<option value = "3" >512 GB</option >' +
                                '<option value = "4" >1 TB</option >';
                                break;
                        }

                        html += '</select >' +

                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Tipo de Disco</h6>' +

                        '<select id="tipo_discoM" class="form-control">' +
                            '<option value = "' + item.id_type_disk + '" >' + item.name_type_disk + '</option >';
                    switch (item.id_type_disk) {
                        case ("1"):
                            html += '<option value = "2" >HDD</option >' +
                                '<option value = "3" >N/A</option >';
                            break;
                        case ("2"):
                            html += '<option value = "1" >SSD</option >' +
                                '<option value = "3" >N/A</option >';
                            break;
                        
                        default:
                            html += '<option value = "1" >SSD</option >' +
                                '<option value = "2" >HDD</option >' +
                                '<option value = "3" >N/A</option >' ;
                            break;
                    }

                    html += '</select >' +

                        '</div>' +
                        '<div class="col-sm-3">' +
                        '<h6>Ram</h6>' +
                        '<select id="ramM" class="form-control">' +
                        '<option value = "' + item.id_ram_memory + '" >' + item.ram_memoria + '</option >';
                    switch (item.id_ram_memory) {
                            case ("1"):
                                html += '<option value = "2" >16</option >' +
                                    '<option value = "3" >0</option >';
                                break;
                            case ("2"):
                                html += '<option value = "1" >8</option >' +
                                    '<option value = "3" >0</option >';
                                break;

                            default:
                                html += '<option value = "1" >8</option >' +
                                    '<option value = "2" >16</option >' +
                                    '<option value = "3" >0</option >';
                                break;
                        }

                    html += '</select>' +
                            '</div>' + 
                            '<div class="col-sm-6">' +
                                '<br></br><h6>No. Activo Fijo</h6>' +
                                '<input type="textarea" id="activo_fijo" name="activo_fijo" class="form-control" value="' + item.activo_fijo + '"  />' +
                            '</div>' +
                            '<div class="col-sm-6">' +
                                '<br></br><h6>Clave de Bitlocker</h6>' +
                                '<input type="textarea" id="bitlocker_2" name="bitlocker_2" class="form-control" value="' + item.bitlocker + '"  />' +
                            '</div>' +
                            '<div class="col-sm-12">' +
                                '<br></br><h6>Comentarios</h6>' +
                                '<input type="textarea" id="comments_2" name="comments_2" class="form-control" value="' + item.comments + '"  />' +
                            '</div>' +
                        '</center>';

                    
                });
                $('.datos').html(html);
            },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;  
}
//Function for getting the Data Based upon Employee ID
function getbyHistoryc_Equip(id) {
   
    $.ajax({
        url: "/siixsem_it/Get_HistorycEquip/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success:
            function (result) {
                var html = '';
                $.each(result, function (key, item) {
                    html += '<tr class="fila row_table" id="fila">';
                    html += '<td><center>' + item.username + '</center></td>';
                    html += '<td><center>' + item.no_nomi + '</center></td>';
                    html += '<td><center>' + item.e_status + '</center></td>';
                    if (item.name_file != "") {
                        html += '<td>' +
                            '<form action="Dowland_Responsive" method="post">' +
                            '<input value="' + item.name_file + '" type="hidden" name="name_file" />' +
                            '<center><button class="fa  fa-download btn-sm btn btn-default " style="background:#5CE1E6;"   type="submit"></button></center>' +
                            '</form>' +
                            '</td>';
                    } else {
                        html += '<td></td>';
                    }//html += '<td><center><input type="submit" value="Enviar" /></center></td>';
                    //html += '<td><center><button style="background:#FFDE59;" class="btn btn-sm btn-default mt-2 " data-toggle="modal" data-target="#edit_equipment" data-id="' + item.id + '"><i class="fas fa-bars"></i></button></center></td>';
                    //html += "<td><button class='btn btn-sm btn-primary mt-2' data-toggle='modal' data-target='#edit-employee-modal' data-id='"+value.id+"'>Edit</button></td>";
                    // html += '<td><button type="button" class="btn btn-default  fa fa-pencil" data-toggle="modal" data-target="#myModal" style="background:#ffde59; " onclick="return get(' + item.id_employe + ')"></button></td>';

                    // html += '<td><button type="button" class="btn btn-default  fa fa-pencil" data-toggle="modal" data-target="#myModal" style="background:#ffde59; " onclick="return getbyID(' + item.id_ticket + ')"></button></td>';

                    html += '</tr>';
                });
                $('.tbody_histo').html(html);
            },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}
function Update_equipment() {
    var comments = $('#comments_2').val();
    var id = $('#idModal').val();
    var ubicacion = $('#ubicacionM').val();
    var status = $('#statusM').val();
    var id_procesadorM = $('#id_procesadorM').val();
    var disk_capacity = $('#almacenamientooM').val();
    console.log("Almacenamiento id   " + disk_capacity);
    var id_disk_type = $('#tipo_discoM').val();
    var ramM = $('#ramM').val();
    var id_code_e = $('#cod_descriptionM').val();
    var activo_fijo = $('#activo_fijo').val();
    var bitlocker = $('#bitlocker_2').val();
    
    $.ajax({
        url: "/siixsem_it/update_equipment_it",
        type: "POST",
        data: {
            id:id,
            comments: comments,
            e_status: status,
            ubication: ubicacion,
            activo_fijo: activo_fijo,
            bitlocker: bitlocker,
            id_procesador: id_procesadorM,
            disk_capacity: disk_capacity, 
            id_type_disk: id_disk_type,
            id_ram_memory: ramM,
            id_code_e: id_code_e
        },
        cache: false,
        beforeSend:
            function (cargando) {
            },
        success: function (result) {
            alertify.notify('¡Se actualizo información del dispositivo en el inventario con éxito!', 'success', 3, function () { location.reload(); });
            
        },

        error: function (errormessage) {
            alertify.error('Upss... Ocurrio un error comunicate al departamento de IT ¡Gracias!');
        }
    });
}
function fnExcelReport() {
    var tab_text = "<meta http-equiv='content-type' content='application/vnd.ms-excel; charset=UTF-8'>" +
        "<table border='2px'>" + "<tr bgcolor='#5ab8ff'>";
    var j = 0;
    tab = document.getElementById('table'); // id of table
    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";

    }
    var ua = window.navigator.userAgent;
    sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));
    return (sa);
}

