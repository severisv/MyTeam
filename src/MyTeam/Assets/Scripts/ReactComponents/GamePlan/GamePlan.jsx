var React = require('react');

module.exports = React.createClass({
    getInitialState: function () {
        return ({

            rows: [
                {
                    lw: 'player',
                    s: 'player',
                    rw: 'player',
                    lcm: 'player',
                    rcm: 'player',
                    dm: 'player',
                    lb: 'player',
                    lcb: 'player',
                    rcb: 'player',
                    rb: 'player',
                    gk: 'player'
                }
            ]
        });
    },

    setPlayer: function (i, key, input) {
        var rows = this.state.rows;
        rows[i][key] = input.target.value;
        this.setState({
            rows: rows
        });
    },

    render: function () {
        var that = this;
        return (
            <div className='mt-container gameplan'>
                {this.state.rows.map(function (lineup, i) {
                    return (<div key={i}>
                        <div className='gp-row'>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lw')} value={lineup.lw} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 's')} value={lineup.s} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rw')} value={lineup.rw} /></div>
                        </div>
                        <div className='gp-row'>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                        </div>
                        <div className='gp-row'>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lcm')} value={lineup.lcm} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rcm')} value={lineup.rcm} /></div>
                            <div className='gp-square'></div>
                        </div>
                        <div className='gp-row'>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'dm')} value={lineup.dm} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                        </div>
                        <div className='gp-row'>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'lb')} value={lineup.lb} /></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rcb')} value={lineup.rcb} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rcb')} value={lineup.rcb} /></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'rb')} value={lineup.rb} /></div>
                        </div>
                        <div className='gp-row'>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'><input onChange={that.setPlayer.bind(null, i, 'gk')} value={lineup.gk} /></div>
                            <div className='gp-square'></div>
                            <div className='gp-square'></div>
                        </div>
                    </div>);
                })}
            </div>


            );
    }
});

