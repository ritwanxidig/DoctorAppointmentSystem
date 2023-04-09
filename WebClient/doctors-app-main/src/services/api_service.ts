import { Doctor, Specialty } from "../models/doctor";

const API_URL = "https://localhost:7181/";

export class api_service {
  // static async getDoctors(
  //   url: string,
  //   page: number,
  //   size: number
  // ): Promise<Doctor[]> {
  //   const response = await fetch(API_URL + url);
  //   const data = await response.json();
  //   return data as Doctor[];
  // }

  // static async getSpecialties(): Promise<Specialty[]> {
  //   const url = `${API_URL}Doctors/specialties`;
  //   const response = await fetch(url);
  //   const data = (await response.json());

  //   return data as Specialty[];
  // }

  static async getData<T>(url: String, queryParams?: {}) {
    const query = new URLSearchParams();
    if (queryParams) {
      const keys = Object.keys(queryParams);
      for (const key of keys) {
        query.append(key, (queryParams as any)[key]);
      }
    }
    url += "?" + query.toString();
    const response = await fetch(API_URL + url);
    const data = await response.json();
    return data as T[];
  }
}
