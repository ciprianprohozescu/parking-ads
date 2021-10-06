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
    const response = await fetch('parkingSpots');
    const data = await response.json();
    this.setState({ parkingSpots: data, loading: false });
  }
}
