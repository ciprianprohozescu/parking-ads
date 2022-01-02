import React, { Component } from 'react';
import axios from 'axios';

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
            <th>Coord.</th>
            <th>Name</th>
            <th>Available</th>
            <th>Max</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {parkingSpots.map(parkingSpot =>
            <tr key={parkingSpot.name}>
              <td>{parkingSpot.coord}</td>
              <td>{parkingSpot.name}</td>
              <td>{parkingSpot.current}</td>
              <td>{parkingSpot.max}</td>
              <td>{parkingSpot.date}</td>
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
    const data = await axios.get('http://localhost:5001/parking');
    this.setState({ parkingSpots: data.data, loading: false });
  }
}
