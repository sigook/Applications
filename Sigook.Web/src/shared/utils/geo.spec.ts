import { describe, expect, it } from 'vitest';
import { cardinalDirection, distanceInMeters, formatDistance } from '@/shared/utils/geo';

const TORONTO = { lat: 43.6532, lon: -79.3832 };
const MISSISSAUGA = { lat: 43.589, lon: -79.6441 };

describe('distanceInMeters', () => {
  it('returns 0 for the same point', () => {
    expect(distanceInMeters(TORONTO.lat, TORONTO.lon, TORONTO.lat, TORONTO.lon)).toBe(0);
  });

  it('computes the distance between two cities', () => {
    const meters = distanceInMeters(TORONTO.lat, TORONTO.lon, MISSISSAUGA.lat, MISSISSAUGA.lon);
    expect(meters).toBeGreaterThan(21500);
    expect(meters).toBeLessThan(22500);
  });

  it('is symmetric', () => {
    const forward = distanceInMeters(TORONTO.lat, TORONTO.lon, MISSISSAUGA.lat, MISSISSAUGA.lon);
    const backward = distanceInMeters(MISSISSAUGA.lat, MISSISSAUGA.lon, TORONTO.lat, TORONTO.lon);
    expect(forward).toBeCloseTo(backward, 6);
  });
});

describe('formatDistance', () => {
  it.each([
    [0, '0 m'],
    [99.6, '100 m'],
    [999.4, '999 m'],
    [1000, '1.0 km'],
    [9949, '9.9 km'],
    [10000, '10 km'],
    [21780, '22 km'],
  ])('formats %d meters as %s', (meters, expected) => {
    expect(formatDistance(meters)).toBe(expected);
  });
});

describe('cardinalDirection', () => {
  it.each([
    [1, 0, 'north'],
    [1, 1, 'north-east'],
    [0, 1, 'east'],
    [-1, 1, 'south-east'],
    [-1, 0, 'south'],
    [-1, -1, 'south-west'],
    [0, -1, 'west'],
    [1, -1, 'north-west'],
  ])('offset (%d, %d) points %s', (dLat, dLon, expected) => {
    expect(cardinalDirection(0, 0, dLat, dLon)).toBe(expected);
  });
});
