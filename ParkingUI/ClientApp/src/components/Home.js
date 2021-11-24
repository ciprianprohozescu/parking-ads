import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Hello, driver!</h1>
        <p>Welcome to ParkingAds. The only parking solutuion to work everywhere in the Milky Way</p>
      </div>
    );
  }
}
