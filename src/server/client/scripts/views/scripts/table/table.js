$('.table-table')
  .find('td')
  .each(function f() {
    const content = this.innerText
    if (content.indexOf('Wam-Kam') > -1) {
      $(this)
        .parent('tr')
        .addClass('team-primary')
    }
  })
