<template>
  <div class="home">
    <section class="hero">
      <div class="hero-content">
        <h1>给毛孩子最好的宠爱</h1>
        <p class="hero-sub">专业宠物洗护 · 在线预约 · 到店即享</p>
        <div class="hero-actions">
          <router-link to="/booking" class="btn-primary">🐱 立即预约</router-link>
          <router-link to="/services" class="btn-outline">查看服务</router-link>
        </div>
      </div>
      <div class="hero-illustration">
        <span class="hero-pet">🐶 🐱 🐾</span>
      </div>
    </section>

    <section class="section">
      <h2 class="section-title">热门服务</h2>
      <p class="section-desc">专业洗护师，用心对待每一只宠物</p>
      <div class="service-cards">
        <div class="service-card" v-for="s in featuredServices" :key="s.name">
          <div class="service-icon">{{ s.icon }}</div>
          <h3>{{ s.name }}</h3>
          <p>{{ s.desc }}</p>
          <div class="service-meta">
            <span>⏱ {{ s.duration }}</span>
            <span class="price">¥{{ s.price }}</span>
          </div>
        </div>
      </div>
    </section>

    <section class="section features">
      <h2 class="section-title">为什么选择我们</h2>
      <div class="feature-grid">
        <div class="feature-item" v-for="f in features" :key="f.title">
          <div class="feature-icon">{{ f.icon }}</div>
          <h3>{{ f.title }}</h3>
          <p>{{ f.desc }}</p>
        </div>
      </div>
    </section>

    <section class="section cta-section">
      <div class="cta-card">
        <h2>准备好给毛孩子一个焕然一新的体验了吗？</h2>
        <p>首次预约享 8 折优惠</p>
        <router-link to="/booking" class="btn-primary btn-lg">立即预约 →</router-link>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
const featuredServices = [
  { icon: '🛁', name: '精致洗护', desc: '全身深层清洁 + 护毛精华', duration: '60分钟', price: '128' },
  { icon: '✂️', name: '专业美容', desc: '修剪造型 + 耳部护理', duration: '90分钟', price: '198' },
  { icon: '💆', name: 'SPA 水疗', desc: '水疗按摩 + 香薰护理', duration: '75分钟', price: '298' },
  { icon: '🩺', name: '基础护理', desc: '剪指甲 + 清洁耳道 + 脚底毛', duration: '30分钟', price: '68' }
]

const features = [
  { icon: '🏅', title: '持证洗护师', desc: '所有洗护师均持有专业资格证书，经验丰富' },
  { icon: '🌿', title: '天然洗护用品', desc: '选用进口天然宠物洗护产品，温和不刺激' },
  { icon: '📱', title: '在线便捷预约', desc: '随时随地预约，到店即享，无需排队等待' },
  { icon: '💝', title: '一对一服务', desc: '每位宠物专属洗护师，全程贴心陪伴' }
]
</script>

<style scoped>
.hero {
  max-width: 1200px;
  margin: 0 auto;
  padding: 80px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 48px;
}
.hero-content h1 {
  font-size: 48px;
  color: var(--primary);
  line-height: 1.2;
  margin-bottom: 16px;
}
.hero-sub {
  font-size: 18px;
  color: var(--muted);
  margin-bottom: 32px;
}
.hero-actions { display: flex; gap: 16px; }
.hero-pet { font-size: 80px; }
.btn-primary {
  display: inline-block;
  background: var(--secondary);
  color: #fff;
  padding: 14px 32px;
  border-radius: var(--radius-md);
  font-size: 16px;
  font-weight: 600;
  transition: all var(--transition);
}
.btn-primary:hover { background: color-mix(in srgb, var(--secondary) 85%, black); transform: translateY(-1px); }
.btn-outline {
  display: inline-block;
  color: var(--primary);
  padding: 14px 32px;
  border-radius: var(--radius-md);
  font-size: 16px;
  font-weight: 600;
  border: 2px solid var(--primary);
  transition: all var(--transition);
}
.btn-outline:hover { background: var(--primary-bg); }
.section { max-width: 1200px; margin: 0 auto; padding: 64px 24px; }
.section-title { font-size: 32px; text-align: center; color: var(--text); margin-bottom: 8px; }
.section-desc { text-align: center; color: var(--muted); margin-bottom: 40px; }
.service-cards {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
}
.service-card {
  background: var(--card);
  border-radius: var(--radius-md);
  padding: 32px 24px;
  text-align: center;
  box-shadow: var(--shadow-card);
  transition: transform var(--transition);
}
.service-card:hover { transform: translateY(-4px); }
.service-icon { font-size: 48px; margin-bottom: 16px; }
.service-card h3 { font-size: 18px; margin-bottom: 8px; }
.service-card p { color: var(--muted); font-size: 14px; margin-bottom: 16px; }
.service-meta { display: flex; justify-content: center; gap: 16px; font-size: 14px; }
.price { color: var(--secondary); font-weight: 700; }
.features { background: #fff; max-width: 100%; padding: 64px calc((100% - 1200px) / 2 + 24px); }
.feature-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 32px;
}
.feature-item { text-align: center; }
.feature-icon { font-size: 40px; margin-bottom: 12px; }
.feature-item h3 { font-size: 16px; margin-bottom: 8px; }
.feature-item p { color: var(--muted); font-size: 14px; line-height: 1.6; }
.cta-section { padding: 48px 24px; }
.cta-card {
  background: linear-gradient(135deg, var(--primary), #0D9488);
  border-radius: var(--radius-lg);
  padding: 48px;
  text-align: center;
  color: #fff;
}
.cta-card h2 { font-size: 28px; margin-bottom: 8px; }
.cta-card p { opacity: 0.85; margin-bottom: 24px; font-size: 16px; }
.btn-lg { padding: 16px 40px; font-size: 18px; }
@media (max-width: 768px) {
  .hero { flex-direction: column; text-align: center; padding: 48px 16px; }
  .hero-content h1 { font-size: 32px; }
  .hero-pet { font-size: 48px; }
  .service-cards, .feature-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 480px) {
  .service-cards, .feature-grid { grid-template-columns: 1fr; }
}
</style>
