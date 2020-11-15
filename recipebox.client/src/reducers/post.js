import {
	GET_POSTS,
	GET_POST,
	GET_CUISINE,
	DELETE_POST,
	POST_ERROR,
	POST_SUBMIT_SUCCESS,
	POST_SUBMIT_FAIL,
	POST_UPDATE_SUCCESS,
	POST_UPDATE_FAIL,
	RECIPE_PHOTO_DELETED,
	RECIPE_PHOTO_DELETION_ERROR,
	COMMENT_ADDED,
	COMMENT_UPDATED,
	COMMENT_REMOVED
} from '../actions/types';

const initialState = {
	posts: [],
	post: null,
	loading: true,
	error: {},
	storeSearchParams: '',
	storeOrderBy: ''
	// postSubmitted: false
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case GET_POSTS:
		case GET_CUISINE:
			return {
				...state,
				posts: payload.postsToReturn,
				storeSearchParams: payload.searchParams,
				storeOrderBy: payload.orderBy,
				loading: false
			};
		case GET_POST:
			return {
				...state,
				post: payload,
				loading: false
			};
		case POST_SUBMIT_SUCCESS:
		case POST_UPDATE_SUCCESS:
			// case COMMENT_ADDED:
			// case COMMENT_UPDATED:
			return {
				...state,
				loading: false,
				posts: [ payload, ...state.posts ],
				post: payload
				// postSubmitted: true
			};
		case DELETE_POST:
			return {
				...state,
				posts: state.posts.filter((post) => post.postId !== payload),
				loading: false
			};
		case RECIPE_PHOTO_DELETED:
			return {
				...state,
				post: {
					...state.post,
					postPhoto: state.post.postPhoto.filter((photo) => photo.postPhotoId !== payload)
				},
				loading: false
			};
		case COMMENT_ADDED:
		case COMMENT_UPDATED:
			return {
				...state,
				loading: false,
				posts: [ ...state.posts ],
				post: {
					...state.post,
					comments: [ payload, ...state.post.comments ]
				}
			};
		case COMMENT_REMOVED:
			return {
				...state,
				post: {
					...state.post,
					comments: state.post.comments.filter((comment) => comment.commentId !== payload)
				},
				loading: false
			};
		case POST_SUBMIT_FAIL:
		case POST_UPDATE_FAIL:
		case POST_ERROR:
		case RECIPE_PHOTO_DELETION_ERROR:
			return {
				...state,
				error: payload,
				loading: false
			};

		default:
			return state;
	}
}
