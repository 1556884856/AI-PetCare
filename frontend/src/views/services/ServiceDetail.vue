<template>
  <div class="page" v-loading="loading">
    <div class="breadcrumb">
      <router-link to="/services">← 返回服务列表</router-link>
    </div>
    <div class="detail" v-if="service">
      <div class="detail-img">{{ service.petType === 'Cat' ? '🐈' : '🐕' }}</div>
      <div class="detail-info">
        <el-tag>{{ categoryLabel }}</el-tag>
        <h1>{{ service.name }}</h1>
        <p class="desc">{{ service.description }}</p>
        <div class="meta">
          <div class="meta-item"><span class="label">时长</span><span>⏱ {{ service.durationMinutes }} 分钟</span></div>
          <div class="meta-item"><span class="label">适用</span><span>{{ service.petType === 'All' ? '🐕🐈 猫狗通用' : service.petType === 'Dog' ? '🐕 狗狗' : '🐈 猫咪' }}</span></div>
        </div>
        <div class="price-row">
          <span class="price">¥{{ service.price }}</span>
          <el-button type="warning" size="large" round @click="$router.push(`/booking?serviceId=${service.id}`)">立即预约</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getServiceDetail } from '@/api/services'

const route = useRoute()
const service = ref<any>(null)
const loading = ref(false)

const categoryLabel = computed(() => {
  const m: any = { Bath: '洗浴', Grooming: '美容', Spa: 'SPA', Basic: '基础护理' }
  return m[service.value?.category] || service.value?.category
})

onMounted(async () => {
  loading.value = true
  try {
    const res: any = await getServiceDetail(Number(route.params.id))
    service.value = res.data
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.page { max-width: 1200px; margin: 0 auto; padding: 48px 24px; }
.breadcrumb { margin-bottom: 32px; }
.breadcrumb a { color: var(--primary); font-size: 15px; }
.detail { display: flex; gap: 48px; background: #fff; border-radius: var(--radius-md); padding: 40px; box-shadow: var(--shadow-card); }
.detail-img {
  font-size: 100px;
  background: var(--primary-bg);
  border-radius: var(--radius-md);
  width: 240px;
  height: 240px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.detail-info h1 { font-size: 28px; margin: 12px 0; }
.desc { color: var(--muted); line-height: 1.7; margin-bottom: 24px; }
.meta { display: flex; gap: 32px; margin-bottom: 24px; }
.meta-item .label { display: block; color: var(--muted); font-size: 13px; margin-bottom: 4px; }
.price-row { display: flex; align-items: center; gap: 24px; }
.price { font-size: 32px; color: var(--secondary); font-weight: 700; }
@media (max-width: 768px) { .detail { flex-direction: column; padding: 24px; } .detail-img { width: 100%; height: 160px; } }
</style>
