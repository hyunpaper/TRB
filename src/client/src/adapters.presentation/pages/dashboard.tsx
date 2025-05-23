import { useEffect, useState } from "react";
import { getMinuteCandles } from "../../adapters.persistence/apis/upbit";
import CoinChart from "../components/CoinChart";
import AuthPage from "../pages/AuthPage";
import RegisterPopup from "./RegisterPopup";
import { priceSocketService } from "../../adapters.infrastructure/sockets/PriceSocketService";
import { useAuth } from "../hooks/AuthContext";
import EditProfilePopup from "../components/EditProfilePopup";

const markets = [
  "KRW-BTC", "KRW-ETH", "KRW-XRP", "KRW-DOGE", "KRW-SOL",
  "KRW-ADA", "KRW-DOT", "KRW-AVAX", "KRW-LINK", "KRW-TRX"
];

export default function DashboardPage() {
  const [chartData, setChartData] = useState<Record<string, any[]>>({});
  const [showRegister, setShowRegister] = useState(false);
  const [showEditProfile, setShowEditProfile] = useState(false);
  const { isLoggedIn, email, role, nickname, profileImage, logout, user, fetchProfile } = useAuth();
  const [profileLoaded, setProfileLoaded] = useState(false);

    // 이미지 모달 상태
  const [showImageModal, setShowImageModal] = useState(false);
  const [modalImageUrl, setModalImageUrl] = useState("");

  // 이미지 클릭 핸들러
  const handleImageClick = (url: string) => {
    setModalImageUrl(url);
    setShowImageModal(true);
  };

    const closeImageModal = () => {
    setShowImageModal(false);
    setModalImageUrl("");
  };


  useEffect(() => {
    const fetchAll = async () => {
      const results: Record<string, any[]> = {};
      for (const market of markets) {
        try {
          const data = await getMinuteCandles(market);
          results[market] = data;
          await new Promise((res) => setTimeout(res, 200));
        } catch (err) {
          console.error(`${market} 불러오기 실패`, err);
        }
      }
      setChartData(results);
    };
    fetchAll();
  }, []);

  useEffect(() => {
    if (!priceSocketService.isReady()) {
      priceSocketService.connect(markets);
    }
  }, []);
  
useEffect(() => {
  if (isLoggedIn) {
    fetchProfile()
      .then(() => {
        console.log("✅ 프로필 정보 불러옴");
        setProfileLoaded(true); // ✅ 여기에 표시
      })
      .catch(() => {
        console.error("❌ 프로필 불러오기 실패");
        setProfileLoaded(false);
      });
  }
}, [isLoggedIn]);
  return (
    <div className="flex h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-gray-200">
      <div className="flex-1 overflow-y-auto px-8 py-6">
        <h1 className="text-2xl font-bold text-gray-800 mb-4 tracking-tight">📊 코인 차트</h1>
        <div className="grid grid-cols-1 gap-5">
          {Object.entries(chartData).map(([market, candles]) => (
            <div
              key={market}
              className="bg-white p-5 rounded-xl shadow-md border border-gray-200 hover:shadow-lg transition"
            >
              <CoinChart market={market} candles={candles} />
            </div>
          ))}
        </div>
      </div>

      <div className="w-[360px] bg-white shadow-2xl border-l px-6 py-8 flex flex-col justify-between">
        {!isLoggedIn ? (
          <div>
            <h2 className="text-2xl font-semibold text-center mb-2">🔐 로그인</h2>
            <p className="text-center text-gray-500 text-sm mb-6">TRB 서비스를 사용하려면 로그인하세요.</p>
            <div className="border-t border-gray-200 mb-4" />
            <AuthPage mode="panel" onShowRegister={() => setShowRegister(true)} />
          </div>
        ) : (
          <div className="text-center space-y-4">
              {/* 프로필 이미지 */}
              {profileImage && (
                <div className="flex justify-center">
                  <img
                    src={`http://localhost:5186${profileImage}`}
                    alt="프로필 이미지"
                    className="w-48 h-48 rounded-full border-2 border-gray-300 object-cover cursor-pointer"
                    onClick={() => handleImageClick(`http://localhost:5186${profileImage}`)}
                  />
                </div>
              )}

              {/* 텍스트 정보 */}
              <div>
                <h2 className="text-2xl font-semibold mt-2">👋 환영합니다!</h2>
                <p className="text-gray-600 text-sm mt-1">
                  {email}
                  {nickname && <> [<span className="font-semibold">{nickname}</span>]</>}
                  {role && <> · <span className="font-semibold">{role}</span></>}
                </p>
              </div>
              {/* 프로필 수정 버튼 */}
              <div className="flex flex-col gap-2">
                <button
                  onClick={() => setShowEditProfile(true)}
                  className="w-full bg-gray-200 hover:bg-gray-300 text-gray-800 py-2 rounded"
                >
                  프로필 수정
                </button>
                <button
                  onClick={logout}
                  className="w-full bg-red-500 hover:bg-red-600 text-white py-2 rounded"
                >
                  로그아웃
                </button>
              </div>
            </div>
        )}
        <footer className="text-center text-gray-400 text-xs mt-8">
          &copy; {new Date().getFullYear()} TRB. All rights reserved.
        </footer>
      </div>

      {showRegister && (
        <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex justify-center items-center">
          <RegisterPopup onClose={() => setShowRegister(false)} />
        </div>
      )}
      {showImageModal && (
        <div
          className="fixed inset-0 z-50 bg-black bg-opacity-60 flex items-center justify-center cursor-pointer transition-opacity duration-300"
          onClick={closeImageModal}
        >
          <img
            src={modalImageUrl}
            alt="확대된 프로필 이미지"
            className="max-w-[80%] max-h-[80%] rounded-full border-1 border-black shadow-2xl object-cover transform scale-95 transition-transform duration-300 hover:scale-100"
          />
        </div>
      )}
    {showEditProfile && user && (
      <EditProfilePopup
        email={email ?? ""}
        name={user.name}
        nickname={user.nickname}
        birthDate={user.brith}
        gender={user.gender}
        address={user.address}
        profileImage={user.profileImage}
        onClose={() => setShowEditProfile(false)}
        onSuccess={async () => {
          await fetchProfile();
        }}
      />
    )}
    </div>
  );
}