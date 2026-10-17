const EARTH_RADIUS_METERS = 6376500;
const CARDINAL_DIRECTIONS = ['north', 'north-east', 'east', 'south-east', 'south', 'south-west', 'west', 'north-west'];

const toRadians = (degrees: number): number => (degrees * Math.PI) / 180;
const toDegrees = (radians: number): number => (radians * 180) / Math.PI;

export function distanceInMeters(latitude1: number, longitude1: number, latitude2: number, longitude2: number): number {
  const lat1 = toRadians(latitude1);
  const lon1 = toRadians(longitude1);
  const lat2 = toRadians(latitude2);
  const deltaLon = toRadians(longitude2) - lon1;
  const a = Math.sin((lat2 - lat1) / 2) ** 2 + Math.cos(lat1) * Math.cos(lat2) * Math.sin(deltaLon / 2) ** 2;
  return EARTH_RADIUS_METERS * (2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a)));
}

export function cardinalDirection(fromLatitude: number, fromLongitude: number, toLatitude: number, toLongitude: number): string {
  const lat1 = toRadians(fromLatitude);
  const lat2 = toRadians(toLatitude);
  const deltaLon = toRadians(toLongitude - fromLongitude);
  const y = Math.sin(deltaLon) * Math.cos(lat2);
  const x = Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(deltaLon);
  const bearing = (toDegrees(Math.atan2(y, x)) + 360) % 360;
  return CARDINAL_DIRECTIONS[Math.round(bearing / 45) % 8];
}
