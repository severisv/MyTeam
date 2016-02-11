
var EmailAdd = React.createClass({
   
    getInitialState: function () {
        return { firstname: '', lastname: '', email: '' };
    },
    
    submit: function(){
        var user = {        
            first_name: this.state.firstname,
            last_name: this.state.lastname,
            email: this.state.email            
        }
        this.props.addPlayer(user);
    },

    handleFirstnameChange: function (event) { this.setState({ firstname: event.target.value }) },
    handleLastnameChange: function (event) { this.setState({ lastname: event.target.value }) },
    handleEmailChange: function (event) { this.setState({ email: event.target.value }) },

    render: function () {
        return (
                  <div>
                       <span className="text-danger">{this.props.validationMessage}</span>
                      <input onChange={this.handleEmailChange} className="form-control" placeholder="E-postadresse" />
                      <input onChange={this.handleFirstnameChange} className="form-control" placeholder="Fornavn" />
                      <input onChange={this.handleLastnameChange} className="form-control" placeholder="Etternavn" />
                      <button onClick={this.submit}  className="btn btn-primary"><i className="fa fa-plus"></i>&nbsp;Legg til</button>
                  </div>)
    }
   
        

});


