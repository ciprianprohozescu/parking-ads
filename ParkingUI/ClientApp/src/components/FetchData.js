import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { parkingSpots: [], loading: true };
  }

  componentDidMount() {
    this.populateParkingSpotsData();
  }

  static renderParkingSpotsTable(parkingSpots) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Date</th>
            <th>Name</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {parkingSpots.map(parkingSpot =>
            <tr key={parkingSpot.date}>
              <td>{parkingSpot.date}</td>
              <td>{parkingSpot.name}</td>
              <td>{parkingSpot.summary}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderParkingSpotsTable(this.state.parkingSpots);

    return (
      <div>
        <h1 id="tabelLabel" >Parking Spots</h1>
        <p>This component shows available parking spots.</p>
        {contents}
      </div>
    );
  }

  async populateParkingSpotsData() {
    console.log('ok');
    const response1 = fetch('http://localhost:5001/parking');
    console.log('1');
    const response2 = fetch('http://localhost:5001/parking/test');
    console.log('2');
    const response3 = fetch('http://localhost:5001/parking');
    console.log('3');
    const response4 = fetch('http://localhost:5001/parking/test');
    console.log('4');
    const response5 = fetch('http://localhost:5001/parking/test');
    console.log(await Promise.all([response1, response2, response3, response4, response5]))
    // this.setState({ parkingSpots: data, loading: false });
  }
}
