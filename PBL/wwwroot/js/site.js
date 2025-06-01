// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function aplicarFiltroConsulta() {
    var vNome = document.getElementById('nome').value;
    var vValorMenor = document.getElementById('valorMenor').value || null;
    var vValorMaior = document.getElementById('valorMaior').value || null;
    var vDataInicial = document.getElementById('dataInicial').value || null;
    var vDataFinal = document.getElementById('dataFinal').value || null;    $.ajax({
        url: "/Dispositivo/ObterDadosConsulta",
        data: { nome: vNome, valorMenor: vValorMenor, valorMaior: vValorMaior, dataInicial: vDataInicial, dataFinal: vDataFinal },
        success: function (dados) {
            if (dados.erro != undefined) {
                alert(dados.msg);
            }
            else {
                document.getElementById('resultadoConsulta').innerHTML = dados;
            }
        },
    });
}