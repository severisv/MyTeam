$('#leagueTable').find('td').each(function() {
    
    var content = this.innerText;
    if (content.indexOf("Wam-Kam") > -1) {
        $(this).parent('tr').addClass("team-primary");
    }
});

