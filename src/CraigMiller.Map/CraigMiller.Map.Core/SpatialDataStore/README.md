# SpatialDataStore file format

## Overview
File format for storing spatial data in a compact binary format. The file consists of a header followed by a sequence of records, each representing a spatial feature.

## File Structure
All data is stored in little-endian format.

### Header
| Field Name      | Size (bytes) | Description                             |
|-----------------|--------------|-----------------------------------------|
| Magic number    | 4            | Always 'GSDS' (GeoSpatial Data Store)   |
| Version         | 2 (u16)      | File format version (e.g., 1)           |

### Data block
| Field Name      | Size (bytes) | Description                                          |
|-----------------|--------------|------------------------------------------------------|
| Size            | 4 (u32)      | Size of the data block in bytes                      |
| Min zoom level  | 1            | Minimum zoom level for the data                      |
| Max zoom level  | 1            | Maximum zoom level for the data                      |
| Bounding box    | 16 (4 x f32) | Bounding box of the data (minX, minY, maxX, maxY)    |
| Compression     | 1            | Content compression type (0 = none)                  |
| Content type    | 1            | Type of content (0 = deleted, 1 = RTree)             |
| Next data block | 8 (u64)      | File offset to the next data block (0 if none)       |
| Data ID         | 2 (u16)      | User defined ID for the data block (e.g. layer ID)   |
| Content         | Size bytes   | The data block                                       |

### RTree

#### Node
| Field Name      | Size (bytes) | Description                                  |
|-----------------|--------------|----------------------------------------------|
| Bounding box    | 16 (4 x f32) | Bounding box of the node                     |
| Num entries     | 2 (u16)      | Number of entries in the node  	            |
| Entries         | Entries * 4  | Array of entry pointers (child node offsets) |

#### Leaf

### Record types

#### Shape
| Field Name      | Size (bytes) | Description                                   |
|-----------------|--------------|-----------------------------------------------|
| Type            | 1            | Shape type (0 = point, 1 = line, 2 = polygon) |
| Num points      | 4 (u32)      | Number of points in the shape                 |
| Metadata length | 2 (u16)      | Length of the metadata in bytes               |
| Points          | Num pts * 8  | Array of points (X and Y as floats)           |
| Metadata        | Metadata len | Optional metadata bytes                       |

#### Point
| Field Name      | Size (bytes) | Description                             |
|-----------------|--------------|-----------------------------------------|
| X               | 4 (f32)      | X coordinate                            |
| Y               | 4 (f32)      | Y coordinate                            |

#### Bounding box
| Field Name      | Size (bytes) | Description                             |
|-----------------|--------------|-----------------------------------------|
| Min X           | 4 (f32)      | Minimum X coordinate                    |
| Min Y           | 4 (f32)      | Minimum Y coordinate                    |
| Max X           | 4 (f32)      | Maximum X coordinate                    |
| Max Y           | 4 (f32)      | Maximum Y coordinate                    |
