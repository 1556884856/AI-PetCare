<!--
  服务项目列表页
  支持按类别（洗浴/美容/SPA/基础护理）和宠物类型（狗/猫）筛选
  点击卡片可跳转到详情页
-->
<template>
  <div class="page"><h1 class="page-title">🛁 服务项目</h1><p class="page-desc">选择最适合毛孩子的洗护服务</p>
    <div class="filters">
      <el-radio-group v-model="filterCategory" @change="fetchServices">
        <el-radio-button value="">全部</el-radio-button><el-radio-button value="Bath">🛁 洗浴</el-radio-button><el-radio-button value="Grooming">✂️ 美容</el-radio-button><el-radio-button value="Spa">💆 SPA</el-radio-button><el-radio-button value="Basic">🩺 基础护理</el-radio-button>
      </el-radio-group>
      <el-radio-group v-model="filterPetType" @change="fetchServices" style="margin-left: 16px;">
        <el-radio-button value="">全部宠物</el-radio-button><el-radio-button value="Dog">🐕 狗狗</el-radio-button><el-radio-button value="Cat">🐈 猫咪</el-radio-button>
      </el-radio-group>
    </div>
    <div class="service-grid" v-loading="loading">
      <div class="service-card" v-for="s in services" :key="s.id" @click="$router.push(`/services/${s.id}`)">
        <div class="card-img">{{ s.petType === 'Cat' ? '🐈' : s.petType === 'Dog' ? '🐕' : '🐾' }}</div>
        <div class="card-body"><div class="card-tag">{{ categoryLabel(s.category) }}</div><h3>{{ s.name }}</h3><p class="card-desc">{{ s.description }}</p>
          <div class="card-footer"><span>⏱ {{ s.durationMinutes }}分钟</span><span class="card-price">¥{{ s.price }}</span></div>
          <el-button type="primary" class="book-btn" @click.stop="$router.push(`/booking?serviceId=${s.id}`)">立即预约</el-button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getServices } from '@/api/services'
const services = ref<any[]>([]); const loading = ref(false); const filterCategory = ref(''); const filterPetType = ref('')
const categoryLabel = (c: string) => ({ Bath: '洗浴', Grooming: '美容', Spa: 'SPA', Basic: '基础护理' } as any)[c] || c
const fetchServices = async () => {
  loading.value = true
  try { const params: any = {}; if (filterCategory.value) params.category = filterCategory.value; if (filterPetType.value) params.petType = filterPetType.value; const res: any = await getServices(params); services.value = res.data || [] } finally { loading.value = false }
}
onMounted(fetchServices)
</script>
<style scoped>
.page { max-width: 1200px; margin: 0 auto; padding: 48px 24px; }
.page-title { font-size: 32px; margin-bottom: 8px; }
.page-desc { color: var(--muted); margin-bottom: 32px; }
.filters { margin-bottom: 32px; display: flex; flex-wrap: wrap; gap: 12px; }
.service-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; }
.service-card { background: #fff; border-radius: var(--radius-md); overflow: hidden; box-shadow: var(--shadow-card); cursor: pointer; transition: transform var(--transition); }
.service-card:hover { transform: translateY(-4px); }
.card-img { height: 120px; background: var(--primary-bg); display: flex; align-items: center; justify-content: center; font-size: 48px; }
.card-body { padding: 20px; }
.card-tag { display: inline-block; background: #FEF3C7; color: #92400E; padding: 2px 12px; border-radius: var(--radius-full); font-size: 12px; margin-bottom: 8px; }
.card-body h3 { font-size: 18px; margin-bottom: 8px; }
.card-desc { color: var(--muted); font-size: 14px; margin-bottom: 12px; }
.card-footer { display: flex; justify-content: space-between; font-size: 13px; margin-bottom: 12px; }
.card-price { color: var(--secondary); font-weight: 700; font-size: 16px; }
.book-btn { width: 100%; }
@media (max-width: 768px) { .service-grid { grid-template-columns: 1fr; } }
</style>
