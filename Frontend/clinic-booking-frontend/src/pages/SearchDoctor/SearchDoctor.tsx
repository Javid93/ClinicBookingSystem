import { useState, useEffect} from "react";
import api from "@/api/axios";
import Layout from "@/components/Layout";
import "./SearchDoctor.css";

type SearchResult = {
  fullName: string;
  clinicName: string;
  specialityName: string;
};

const SearchDoctor = () => {
    const [searchTerm, setSearchTerm] = useState("");
    const [results, setResults] = useState<SearchResult[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    // For entry animation only
    const [initialLoad, setInitialLoad] = useState(true);


  // Show 1s loading animation only on page mount
  useEffect(() => {
    const timer = setTimeout(() => setInitialLoad(false), 100);
    return () => clearTimeout(timer);
  }, []);

  const handleSearch = async () => {
    setLoading(true);
    setError(null);
    setResults([]);

    //Empty search
    if(!searchTerm.trim()) {
      setError("Please enter a name to search.");
      setLoading(false);
      return;
    }
    try {
      const response = await api.get(`/doctors/search?name=${encodeURIComponent(searchTerm)}`);

      await new Promise(resolve => setTimeout(resolve, 1000));
      setResults(response.data);
      setError(null);
    } catch (err: any) {
      if (err.response?.status === 404) {
        setError("No matching doctors found.");
        setResults([]);
      } else {
        setError("An error occurred while searching.");
        setResults([]);
      }
    } finally {
      setLoading(false);
    }
  };

  //Initial page load animation
  if (initialLoad) {
    return (
      <Layout>
        <div className="search-doctor-container">
          <div className="loader" />
        </div>
      </Layout>
    );
  }


  return (
    <Layout>
      <div className="search-doctor-container">
        <h2> 🔍 Search Doctor</h2>
        <div className="flex gap-2 mb-4">
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Enter first or last name"
            className="border px-3 py-2 rounded w-full"
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                e.preventDefault();
                handleSearch()
              }
            }}
          />
          <button
            onClick={handleSearch}
            className="px-4 py-2 bg-blue-600 text-white rounded disabled:opacity-50"
            disabled={loading}
          >
            Search
          </button>
        </div>

        <div className="status-space">
          {loading && (
            <div className="loading-spinner">
              <span className="spinner"></span>
              <p>Searching for doctors...</p>
            </div>
          )}
          {!loading && error && <p className="text-red-500 text-left w-full">{error}</p>}
        </div>


        {results.length > 0 && (
          <ul className="space-y-4 mt-4">
            {results.map((doc, idx) => (
              <li
                key={idx}
                className="p-4 rounded shadow-sm text-white" 
                style={{ backgroundColor: "#2a2a2a"}}
              >
                <p><strong>👨‍⚕️ Name:</strong> {doc.fullName}</p>
                <p><strong>🏥 Clinic:</strong> {doc.clinicName}</p>
                <p><strong>🔬 Speciality:</strong> {doc.specialityName}</p>
              </li>
            ))}
          </ul>
        )}
      </div>
    </Layout>
  );
};

export default SearchDoctor;
